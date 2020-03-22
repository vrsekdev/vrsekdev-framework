using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy
{
    internal class InterceptedMethodBuilder
    {
        private readonly FieldInfo capturedContexts;
        private readonly DelegateWrapperTypeContainer wrapperContainer;
        private readonly TypeBuilder typeBuilder;
        private MethodInfo interceptedMethod;
        private Type returnType;
        private Type[] parameterTypes;

        private Delegate interceptor;
        private MethodInfo interceptorMethod;

        private ILGenerator methodGenerator;
        private LocalBuilder delegateLocal;

        public InterceptedMethodBuilder(
            DelegateWrapperTypeContainer wrapperContainer,
            TypeBuilder typeBuilder,
            FieldInfo capturedContexts)
        {
            this.capturedContexts = capturedContexts;
            this.wrapperContainer = wrapperContainer;
            this.typeBuilder = typeBuilder;
        }

        public void DefineInterceptedMethod(KeyValuePair<int, MethodInterception> methodInterception)
        {
            interceptedMethod = methodInterception.Value.InterceptedMethod;
            interceptor = methodInterception.Value.Interceptor;
            interceptorMethod = interceptor.GetType().GetMethod("Invoke");

            if (!interceptedMethod.IsVirtual)
            {
                throw new ArgumentException("Intercepted method must be virtual.");
            }

            returnType = interceptedMethod.ReturnType;
            parameterTypes = interceptedMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                interceptedMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot,
                returnType,
                parameterTypes);

            methodGenerator = methodBuilder.GetILGenerator();
            delegateLocal = methodGenerator.DeclareLocal(typeof(Delegate));
            methodGenerator.Emit(OpCodes.Ldarg_0);
            methodGenerator.Emit(OpCodes.Ldfld, capturedContexts);
            methodGenerator.Emit(OpCodes.Ldc_I4, methodInterception.Key);
            methodGenerator.Emit(OpCodes.Callvirt, capturedContexts.FieldType.GetMethod("get_Item") ?? throw new MethodAccessException()); // indexer
            methodGenerator.Emit(OpCodes.Stloc, delegateLocal);
            if (interceptedMethod.ReturnType != typeof(void))
            {
                AddInterceptedFunction();
            }
            else
            {
                AddInterceptedAction();
            }
            methodGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, interceptedMethod);

        }

        private void AddInterceptedFunction()
        {
            // TODO: Add caching
            MethodBuilder invokeMethod = wrapperContainer.TypeBuilder.DefineMethod(
                "Invoke_" + Guid.NewGuid(),
                MethodAttributes.Public,
                returnType,
                parameterTypes);
            ILGenerator invokeGenerator = invokeMethod.GetILGenerator();
            invokeGenerator.Emit(OpCodes.Ldarg_0);
            invokeGenerator.Emit(OpCodes.Ldfld, wrapperContainer.TargetField);
            invokeGenerator.Emit(OpCodes.Call, interceptedMethod);
            invokeGenerator.Emit(OpCodes.Ret);

            var baseDelegateLocal = methodGenerator.DeclareLocal(typeof(Delegate));
            var delegateWrapperLocal = methodGenerator.DeclareLocal(wrapperContainer.TypeBuilder);
            methodGenerator.Emit(OpCodes.Ldtoken, typeof(Func<string>));
            methodGenerator.Emit(OpCodes.Ldarg_0);
            methodGenerator.Emit(OpCodes.Ldtoken, interceptedMethod);
            methodGenerator.Emit(OpCodes.Newobj, wrapperContainer.Constructor);
            methodGenerator.Emit(OpCodes.Stloc, delegateWrapperLocal);
            methodGenerator.Emit(OpCodes.Ldloc, delegateWrapperLocal);
            methodGenerator.Emit(OpCodes.Ldtoken, invokeMethod);
            methodGenerator.Emit(OpCodes.Callvirt, wrapperContainer.CreateDelegateMethod);
            methodGenerator.Emit(OpCodes.Stloc, baseDelegateLocal);
            methodGenerator.Emit(OpCodes.Ldloc, delegateLocal);
            methodGenerator.Emit(OpCodes.Ldloc, baseDelegateLocal);
            methodGenerator.Emit(OpCodes.Callvirt, interceptorMethod);
        }

        private void AddInterceptedAction()
        {
            methodGenerator.Emit(OpCodes.Ldloc, delegateLocal);
            methodGenerator.Emit(OpCodes.Callvirt, interceptorMethod);
            if (!interceptedMethod.IsAbstract)
            {
                methodGenerator.Emit(OpCodes.Ldarg_0);
                methodGenerator.Emit(OpCodes.Call, interceptedMethod);
            }
        }
    }
}
