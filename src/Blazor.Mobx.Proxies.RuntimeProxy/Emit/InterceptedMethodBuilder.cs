using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        private bool provideInterceptedTarget;
        private MethodInfo interceptedMethod;
        private Type returnType;
        private Type[] parameterTypes;

        private MethodInfo interceptorMethod;
        private object interceptorTarget;
        private ILGenerator methodGenerator;
        private LocalBuilder targetLocal;

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
            Initialize(methodInterception);
            Validate();

            MethodBuilder invokeMethod = AddInvokeMethod();

            Type[] baseMethodParameterTypes = interceptedMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                interceptedMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot,
                returnType,
                baseMethodParameterTypes); // same signature as base method

            methodGenerator = methodBuilder.GetILGenerator();
            targetLocal = methodGenerator.DeclareLocal(typeof(object));
            methodGenerator.Emit(OpCodes.Ldarg_0);
            methodGenerator.Emit(OpCodes.Ldfld, capturedContexts);
            methodGenerator.Emit(OpCodes.Ldc_I4, methodInterception.Key);
            methodGenerator.Emit(OpCodes.Callvirt, capturedContexts.FieldType.GetMethod("get_Item") ?? throw new MethodAccessException()); // indexer
            if (!interceptedMethod.IsAbstract)
            {
                methodGenerator.Emit(OpCodes.Stloc, targetLocal);
                var baseDelegateLocal = methodGenerator.DeclareLocal(typeof(Delegate));
                var delegateWrapperLocal = methodGenerator.DeclareLocal(wrapperContainer.TypeBuilder);
                methodGenerator.Emit(OpCodes.Ldtoken, returnType == typeof(void) ? GetActionType() : GetFuncType());
                methodGenerator.Emit(OpCodes.Ldarg_0);
                methodGenerator.Emit(OpCodes.Ldtoken, interceptedMethod);
                methodGenerator.Emit(OpCodes.Newobj, wrapperContainer.Constructor);
                methodGenerator.Emit(OpCodes.Stloc, delegateWrapperLocal);
                methodGenerator.Emit(OpCodes.Ldloc, delegateWrapperLocal);
                methodGenerator.Emit(OpCodes.Ldtoken, invokeMethod);
                methodGenerator.Emit(OpCodes.Callvirt, wrapperContainer.CreateDelegateMethod);
                methodGenerator.Emit(OpCodes.Stloc, baseDelegateLocal);
                if (interceptorTarget != null)
                {
                    methodGenerator.Emit(OpCodes.Ldloc, targetLocal);
                }
                methodGenerator.Emit(OpCodes.Ldloc, baseDelegateLocal);
                for (int i = 1; i < baseMethodParameterTypes.Length + 1; i++)
                {
                    // push received parameters
                    methodGenerator.Emit(OpCodes.Ldarg, i);
                }
            }
            methodGenerator.Emit(OpCodes.Callvirt, interceptorMethod);
            methodGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, interceptedMethod);
        }

        private MethodBuilder AddInvokeMethod()
        {
            // TODO: Add caching
            MethodBuilder invokeMethod = wrapperContainer.TypeBuilder.DefineMethod(
                "Invoke_" + Guid.NewGuid(),
                MethodAttributes.Public,
                returnType,
                parameterTypes);
            ILGenerator invokeGenerator = invokeMethod.GetILGenerator();
            if (provideInterceptedTarget)
            {
                invokeGenerator.Emit(OpCodes.Ldarg_0);
                invokeGenerator.Emit(OpCodes.Ldfld, wrapperContainer.TargetField);
            }
            for (int i = 1; i < parameterTypes.Length + 1; i++)
            {
                invokeGenerator.Emit(OpCodes.Ldarg, i);
            }
            invokeGenerator.Emit(OpCodes.Call, interceptedMethod);
            invokeGenerator.Emit(OpCodes.Ret);

            return invokeMethod;
        }

        private void Initialize(KeyValuePair<int, MethodInterception> methodInterception)
        {
            provideInterceptedTarget = methodInterception.Value.ShouldProvideInterceptedTarget();
            interceptedMethod = methodInterception.Value.GetInterceptedMethod();
            returnType = interceptedMethod.ReturnType;

            if (!provideInterceptedTarget)
            {
                parameterTypes = new List<Type> { interceptedMethod.DeclaringType }
                    .Concat(interceptedMethod.GetParameters().Select(x => x.ParameterType)).ToArray();
            }
            else
            {
                parameterTypes = interceptedMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            }

            interceptorMethod = methodInterception.Value.GetInterceptorMethod();
            interceptorTarget = methodInterception.Value.GetInterceptorTarget();
        }

        private void Validate()
        {
            /*var interceptorParameters = interceptorMethod.GetParameters();
            if (provideInterceptedTarget && interceptorParameters.Length == 0)
            {
                throw new ArgumentException("ProvideInterceptedTarget is set to true, but there are no parameters on interceptor. One parameter expected.");
            }*/

            if (!interceptedMethod.IsVirtual)
            {
                throw new ArgumentException("Intercepted method must be virtual.");
            }
        }

        private Type GetActionType()
        {
            if (parameterTypes.Length == 0)
            {
                return typeof(Action);
            }

            return Expression.GetActionType(parameterTypes);
        }

        private Type GetFuncType()
        {
            if (parameterTypes.Length == 0)
            {
                return typeof(Func<>).MakeGenericType(returnType);
            }

            return Expression.GetFuncType(parameterTypes.Concat(new[] { returnType }).ToArray());
        }
    }
}
