//#define ENABLE_CACHING

using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy
{
    public static class Helper
    {
        public static Delegate CreateDelegate(Type delegateType, object target, RuntimeMethodHandle methodHandle)
        {
            var methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(methodHandle);
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }
    }

    internal static class RuntimeProxyBuilder
    {
        private readonly static Lazy<ModuleBuilder> module;

        static RuntimeProxyBuilder()
        {
            module = new Lazy<ModuleBuilder>(() =>
            {
                var assemblyName = new AssemblyName("ObservablePropertyRuntimeTypeAssembly");
                AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                return assemblyBuilder.DefineDynamicModule("MainModule");
            });
        }

        public static Type BuildRuntimeType(Type type, MethodInfo getMethod, MethodInfo setMethod, MethodInterceptions methodInterceptions = null)
        {
            return BuildRuntimeTypeInternal(type, getMethod, setMethod, methodInterceptions);
        }

        private static Type BuildRuntimeTypeInternal(Type baseType, MethodInfo getMethod, MethodInfo setMethod, MethodInterceptions methodInterceptions = null)
        {
            TypeBuilder typeBuilder = CreateTypeBuilder(baseType);
            FieldBuilder managerField = AddManagerProperty(typeBuilder);
            FieldBuilder capturedContextsField = AddCapturedContextsField(typeBuilder);

            bool isClass = baseType.IsClass;
            AddConstructors(typeBuilder, managerField, capturedContextsField);
            foreach (var propertyInfo in GetPublicProperties(baseType).Where(x => x.GetMethod.IsAbstract || (isClass && x.GetMethod.IsVirtual)))
            {
                AddProperty(typeBuilder, propertyInfo, managerField, getMethod, setMethod);
            }

            if (methodInterceptions != null)
            {
                DelegateWrapperBuilder delegateWrapperBuilder = AddDelegateWrapperType(typeBuilder, baseType);

                foreach (var methodInterception in methodInterceptions)
                {
                    AddInterceptedMethod(typeBuilder, delegateWrapperBuilder, capturedContextsField, methodInterception);
                }
                delegateWrapperBuilder.TypeBuilder.CreateType();
            }

            return typeBuilder.CreateType();
        }

        private static DelegateWrapperBuilder AddDelegateWrapperType(TypeBuilder typeBuilder, Type type)
        {
            var getMethodInfoFromHandle = typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle) });
            var getTypeFromHandle = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), new[] { typeof(RuntimeTypeHandle) });

            TypeBuilder nestedType = module.Value.DefineType("DelegateWrapper" + Guid.NewGuid(), TypeAttributes.Public | TypeAttributes.Class);

            FieldBuilder delegateTypeField = nestedType.DefineField("delegateType", typeof(Type), FieldAttributes.Private);
            FieldBuilder targetField = nestedType.DefineField("target", typeof(object), FieldAttributes.Private);
            FieldBuilder baseMethodInfoField = nestedType.DefineField("methodInfo", typeof(MethodInfo), FieldAttributes.Private);
            FieldBuilder delegateField = nestedType.DefineField("createdDelegate", typeof(Delegate), FieldAttributes.Public);

            ConstructorBuilder ctorBuilder = nestedType.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                new Type[] { typeof(RuntimeTypeHandle), typeof(object), typeof(RuntimeMethodHandle) });

            ILGenerator ctorGenerator = ctorBuilder.GetILGenerator();
            var baseMethodInfoLocal = ctorGenerator.DeclareLocal(typeof(MethodInfo));
            var delegateTypeLocal = ctorGenerator.DeclareLocal(typeof(Type));

            /*** get Type from RuntimeTypeHandle ***/
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Call, getTypeFromHandle);
            /*** set value of delegate type into local variable ***/
            ctorGenerator.Emit(OpCodes.Stloc, delegateTypeLocal);
            /*** set delegateType field ***/
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldloc, delegateTypeLocal);
            ctorGenerator.Emit(OpCodes.Stfld, delegateTypeField);
            /*** set target field ***/
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_2);
            ctorGenerator.Emit(OpCodes.Stfld, targetField);
            /*** get MethodInfo from RuntimeMethodHandle ***/
            ctorGenerator.Emit(OpCodes.Ldarg_3);
            ctorGenerator.Emit(OpCodes.Call, getMethodInfoFromHandle);
            ctorGenerator.Emit(OpCodes.Castclass, typeof(MethodInfo));
            /*** set value of MethodInfo into local variable ***/
            ctorGenerator.Emit(OpCodes.Stloc, baseMethodInfoLocal);
            /*** set baseMethodInfo field ***/
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldloc, baseMethodInfoLocal);
            ctorGenerator.Emit(OpCodes.Stfld, baseMethodInfoField);
            /*** initialize delegate field to null ***/
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldnull);
            ctorGenerator.Emit(OpCodes.Stfld, delegateField);
            /*** return ***/
            ctorGenerator.Emit(OpCodes.Ret);

            MethodInfo createDelegateHelperMethod = typeof(Helper).GetMethod(nameof(Helper.CreateDelegate));

            MethodBuilder createDelegateMethod = nestedType.DefineMethod(
                "CreateDelegate_" + Guid.NewGuid(),
                MethodAttributes.Public,
                typeof(Delegate),
                new Type[] { typeof(RuntimeMethodHandle) });

            ILGenerator createDelegateGenerator = createDelegateMethod.GetILGenerator();
            {
                var delegateLocal = createDelegateGenerator.DeclareLocal(typeof(Delegate));
                createDelegateGenerator.Emit(OpCodes.Ldarg_0);
                createDelegateGenerator.Emit(OpCodes.Ldfld, delegateTypeField);
                createDelegateGenerator.Emit(OpCodes.Ldarg_0);
                createDelegateGenerator.Emit(OpCodes.Ldarg_1);
                createDelegateGenerator.Emit(OpCodes.Call, createDelegateHelperMethod);
                createDelegateGenerator.Emit(OpCodes.Stloc, delegateLocal);
                createDelegateGenerator.Emit(OpCodes.Ldarg_0);
                createDelegateGenerator.Emit(OpCodes.Ldloc, delegateLocal);
                createDelegateGenerator.Emit(OpCodes.Stfld, delegateField);
                createDelegateGenerator.Emit(OpCodes.Ldloc, delegateLocal);
                createDelegateGenerator.Emit(OpCodes.Ret);
            }

            return new DelegateWrapperBuilder
            {
                TypeBuilder = nestedType,
                Constructor = ctorBuilder,
                CreateDelegateMethod = createDelegateMethod,
                DelegateTypeField = delegateTypeField,
                TargetField = targetField,
                BaseMethodInfoField = baseMethodInfoField,
                DelegateField = delegateField
            };
        }

        private static TypeBuilder CreateTypeBuilder(Type type)
        {
            var typeSignature = type.Name + "_RuntimeImpl_" + Guid.NewGuid();
            TypeBuilder typeBuilder = module.Value.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class,
                    null);
            if (type.IsInterface)
            {
                typeBuilder.AddInterfaceImplementation(type);
            }
            else
            {
                typeBuilder.SetParent(type);
            }
            typeBuilder.AddInterfaceImplementation(typeof(IRuntimeProxy));

            return typeBuilder;
        }

        private static void AddInterceptedMethod(TypeBuilder typeBuilder, DelegateWrapperBuilder delegateWrapperBuilder, FieldInfo capturedContexts, KeyValuePair<int, MethodInterception> methodInterception)
        {
            MethodInfo interceptedMethod = methodInterception.Value.InterceptedMethod;

            Delegate interceptor = methodInterception.Value.Interceptor;
            MethodInfo interceptorMethod = interceptor.GetType().GetMethod("Invoke");

            if (!interceptedMethod.IsVirtual)
            {
                throw new ArgumentException("Intercepted method must be virtual.");
            }

            Type returnType = interceptedMethod.ReturnType;
            Type[] parameterTypes = interceptedMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            MethodInfo createDelegateHelperMethod = typeof(Helper).GetMethod(nameof(Helper.CreateDelegate));

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                interceptedMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot,
                returnType,
                parameterTypes);

            ILGenerator methodGenerator = methodBuilder.GetILGenerator();
            var delegateLocal = methodGenerator.DeclareLocal(typeof(Delegate));
            methodGenerator.Emit(OpCodes.Ldarg_0);
            methodGenerator.Emit(OpCodes.Ldfld, capturedContexts);
            methodGenerator.Emit(OpCodes.Ldc_I4, methodInterception.Key);
            methodGenerator.Emit(OpCodes.Callvirt, capturedContexts.FieldType.GetMethod("get_Item") ?? throw new MethodAccessException()); // indexer
            methodGenerator.Emit(OpCodes.Stloc, delegateLocal);
            if (interceptedMethod.ReturnType != typeof(void))
            {
                AddInterceptedFunction(delegateWrapperBuilder, methodGenerator, delegateLocal, interceptedMethod, interceptorMethod);
            }
            else
            {
                AddInterceptedAction(methodGenerator, delegateLocal, interceptedMethod, interceptorMethod);
            }

            methodGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, interceptedMethod);

            
        }

        private static void AddInterceptedFunction(DelegateWrapperBuilder delegateWrapperBuilder, ILGenerator methodGenerator, LocalBuilder delegateLocal, MethodInfo interceptedMethod, MethodInfo interceptorMethod)
        {
            Type returnType = interceptedMethod.ReturnType;
            Type[] parameterTypes = interceptedMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            MethodBuilder invokeMethod = delegateWrapperBuilder.TypeBuilder.DefineMethod(
                "Invoke_" + Guid.NewGuid(),
                MethodAttributes.Public,
                returnType,
                parameterTypes);
            ILGenerator invokeGenerator = invokeMethod.GetILGenerator();
            invokeGenerator.Emit(OpCodes.Ldarg_0);
            invokeGenerator.Emit(OpCodes.Ldfld, delegateWrapperBuilder.TargetField);
            invokeGenerator.Emit(OpCodes.Call, interceptedMethod);
            invokeGenerator.Emit(OpCodes.Ret);

            var baseDelegateLocal = methodGenerator.DeclareLocal(typeof(Delegate));
            var delegateWrapperLocal = methodGenerator.DeclareLocal(delegateWrapperBuilder.TypeBuilder);
            methodGenerator.Emit(OpCodes.Ldtoken, typeof(Func<string>));
            methodGenerator.Emit(OpCodes.Ldarg_0);
            methodGenerator.Emit(OpCodes.Ldtoken, interceptedMethod);
            methodGenerator.Emit(OpCodes.Newobj, delegateWrapperBuilder.Constructor);
            methodGenerator.Emit(OpCodes.Stloc, delegateWrapperLocal);
            methodGenerator.Emit(OpCodes.Ldloc, delegateWrapperLocal);
            methodGenerator.Emit(OpCodes.Ldtoken, invokeMethod);
            methodGenerator.Emit(OpCodes.Callvirt, delegateWrapperBuilder.CreateDelegateMethod);
            methodGenerator.Emit(OpCodes.Stloc, baseDelegateLocal);
            methodGenerator.Emit(OpCodes.Ldloc, delegateLocal);
            methodGenerator.Emit(OpCodes.Ldloc, baseDelegateLocal);
            methodGenerator.Emit(OpCodes.Callvirt, interceptorMethod);
        }

        private static void AddInterceptedAction(ILGenerator methodGenerator, LocalBuilder delegateLocal, MethodInfo interceptedMethod, MethodInfo interceptorMethod)
        {
            methodGenerator.Emit(OpCodes.Ldloc, delegateLocal);
            methodGenerator.Emit(OpCodes.Callvirt, interceptorMethod);
            if (!interceptedMethod.IsAbstract)
            {
                methodGenerator.Emit(OpCodes.Ldarg_0);
                methodGenerator.Emit(OpCodes.Call, interceptedMethod);
            }
        }

        private static void AddConstructors(TypeBuilder typeBuilder, FieldBuilder managerField, FieldBuilder capturedContextsField)
        {
            /*** ctor(manager) ***/
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                new Type[] { managerField.FieldType });

            ILGenerator ctorGenerator = ctorBuilder.GetILGenerator();
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Stfld, managerField);
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldnull);
            ctorGenerator.Emit(OpCodes.Stfld, capturedContextsField);
            ctorGenerator.Emit(OpCodes.Ret);

            /*** ctor(manager, capturedContext) ***/
            ConstructorBuilder ctor2Builder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                new Type[] { managerField.FieldType, capturedContextsField.FieldType });

            // TODO: Capturing a context in a runtime proxy probably creates memory leak
            // because the context can also have a reference to this proxy
            ILGenerator ctor2Generator = ctor2Builder.GetILGenerator();
            ctor2Generator.Emit(OpCodes.Ldarg_0);
            ctor2Generator.Emit(OpCodes.Ldarg_1);
            ctor2Generator.Emit(OpCodes.Stfld, managerField);
            ctor2Generator.Emit(OpCodes.Ldarg_0);
            ctor2Generator.Emit(OpCodes.Ldarg_2);
            ctor2Generator.Emit(OpCodes.Stfld, capturedContextsField);
            ctor2Generator.Emit(OpCodes.Ret);
        }

        private static FieldBuilder AddCapturedContextsField(TypeBuilder typeBuilder)
        {
            return typeBuilder.DefineField(
                "capturedContexts",
                typeof(MethodInterceptions),
                FieldAttributes.Private);
        }

        private static FieldBuilder AddManagerProperty(TypeBuilder typeBuilder)
        {
            Type managerType = typeof(IRuntimeProxyManager);
            string propertyName = nameof(IRuntimeProxy.Manager);

            FieldBuilder managerField = typeBuilder.DefineField(
                "manager",
                managerType,
                FieldAttributes.Private);

            PropertyBuilder managerProperty = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, managerType, null);
            MethodAttributes getAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            MethodBuilder propGetAccessor = typeBuilder.DefineMethod(
                "get_" + propertyName,
                getAttributes,
                managerType,
                Type.EmptyTypes);

            ILGenerator getPropertyGenerator = propGetAccessor.GetILGenerator();
            getPropertyGenerator.Emit(OpCodes.Ldarg_0);
            getPropertyGenerator.Emit(OpCodes.Ldfld, managerField);
            getPropertyGenerator.Emit(OpCodes.Ret);

            managerProperty.SetGetMethod(propGetAccessor);

            return managerField;
        }

        private static void AddProperty(TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldBuilder managerField, MethodInfo getMethod, MethodInfo setMethod)
        {
            Type propertyType = propertyInfo.PropertyType;
            string propertyName = propertyInfo.Name;

            PropertyBuilder propertyBuilder = null;
            if (propertyInfo.DeclaringType.IsInterface)
            {
                propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
            }

            MethodAttributes getSetAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            // Define the "get" accessor method
            MethodBuilder propGetAccessor = typeBuilder.DefineMethod(
                "get_" + propertyName,
                getSetAttributes,
                propertyType,
                Type.EmptyTypes);

            ILGenerator getPropertyGenerator = propGetAccessor.GetILGenerator();
            getPropertyGenerator.Emit(OpCodes.Ldarg_0);
            getPropertyGenerator.Emit(OpCodes.Ldfld, managerField);
            getPropertyGenerator.Emit(OpCodes.Ldstr, propertyName);
            getPropertyGenerator.Emit(OpCodes.Callvirt, getMethod);
            if (propertyInfo.PropertyType.IsValueType)
            {
                getPropertyGenerator.Emit(OpCodes.Unbox, propertyInfo.PropertyType);
                getPropertyGenerator.Emit(OpCodes.Ldobj, propertyInfo.PropertyType);
            }
            getPropertyGenerator.Emit(OpCodes.Ret);

            // Define the "set" accessor method 
            MethodBuilder propSetAccessor = typeBuilder.DefineMethod(
                "set_" + propertyName,
                getSetAttributes,
                null,
                new Type[] { propertyType });

            ILGenerator setPropertyGenerator = propSetAccessor.GetILGenerator();
            setPropertyGenerator.Emit(OpCodes.Ldarg_0);
            setPropertyGenerator.Emit(OpCodes.Ldfld, managerField);
            setPropertyGenerator.Emit(OpCodes.Ldstr, propertyName);
            setPropertyGenerator.Emit(OpCodes.Ldarg_1);
            if (propertyInfo.PropertyType.IsValueType)
            {
                setPropertyGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }
            setPropertyGenerator.Emit(OpCodes.Callvirt, setMethod);
            setPropertyGenerator.Emit(OpCodes.Ret);

            // Map the accessor methods
            if (propertyInfo.DeclaringType.IsInterface)
            {
                propertyBuilder.SetGetMethod(propGetAccessor);
                propertyBuilder.SetSetMethod(propSetAccessor);
            }
            else
            {
                typeBuilder.DefineMethodOverride(propGetAccessor, propertyInfo.GetMethod);
                typeBuilder.DefineMethodOverride(propSetAccessor, propertyInfo.SetMethod);
            }
        }

        private static PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }

        private struct DelegateWrapperBuilder
        {
            public TypeBuilder TypeBuilder { get; set; }
            public ConstructorBuilder Constructor { get; set; }
            public MethodBuilder CreateDelegateMethod { get; set; }
            public FieldBuilder DelegateTypeField { get; set; }
            public FieldBuilder TargetField { get; set; }
            public FieldBuilder BaseMethodInfoField { get; set; }
            public FieldBuilder DelegateField { get; set; }
        }
    }
}
