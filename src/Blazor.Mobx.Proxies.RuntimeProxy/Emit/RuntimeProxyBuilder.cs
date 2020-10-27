using VrsekDev.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Emit
{
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

        public static Type BuildRuntimeType(Type baseType, MethodInfo getMethod, MethodInfo setMethod, MethodInterceptions methodInterceptions = null)
        {
            return BuildRuntimeTypeInternal(baseType, getMethod, setMethod, methodInterceptions);
        }

        private static Type BuildRuntimeTypeInternal(Type baseType, MethodInfo getMethod, MethodInfo setMethod, MethodInterceptions methodInterceptions = null)
        {
            TypeBuilder typeBuilder = CreateTypeBuilder(baseType);
            FieldBuilder managerField = AddManagerProperty(typeBuilder);
            FieldBuilder capturedContextsField = AddCapturedContextsField(typeBuilder);

            bool isClass = baseType.IsClass;
            var properties = GetPublicProperties(baseType).Where(x => x.GetMethod.IsAbstract || (isClass && x.GetMethod.IsVirtual && x.SetMethod != null));
            foreach (var propertyInfo in properties)
            {
                AddProperty(typeBuilder, propertyInfo, managerField, getMethod, setMethod);
            }

            AddConstructors(baseType, typeBuilder, managerField, capturedContextsField, properties);

            if (methodInterceptions != null)
            {
                DelegateWrapperTypeContainer container = new DelegateWrapperBuilder(typeBuilder, baseType).AddDelegateWrapperType();

                var methodBuilder = new InterceptedMethodBuilder(container, typeBuilder, capturedContextsField);
                foreach (var methodInterception in methodInterceptions)
                {
                    methodBuilder.DefineInterceptedMethod(methodInterception);
                }
                container.TypeBuilder.CreateType();
            }

            return typeBuilder.CreateType();
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

        private static void AddConstructors(Type baseType, TypeBuilder typeBuilder, FieldBuilder managerField, FieldBuilder capturedContextsField, IEnumerable<PropertyInfo> properties)
        {
            MethodInfo setDefaultValueMethod = null;
            if (properties.Any() && baseType.IsClass)
            {
                setDefaultValueMethod = managerField.FieldType.GetMethod(nameof(IRuntimeProxyManager.SetDefaultValue));
            }

            ConstructorInfo baseCtor = baseType.GetConstructor(Type.EmptyTypes);

            /*** ctor(manager) ***/
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                new Type[] { managerField.FieldType });

            ILGenerator ctorGenerator = ctorBuilder.GetILGenerator();
            if (baseCtor != null)
            {
                ctorGenerator.Emit(OpCodes.Ldarg_0);
                ctorGenerator.Emit(OpCodes.Call, baseCtor);
            }
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Stfld, managerField);
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldnull);
            ctorGenerator.Emit(OpCodes.Stfld, capturedContextsField);
            if (setDefaultValueMethod != null)
                foreach (var prop in properties)
                {
                    var valueLocal = ctorGenerator.DeclareLocal(prop.PropertyType);
                    ctorGenerator.Emit(OpCodes.Ldarg_0);
                    ctorGenerator.Emit(OpCodes.Call, prop.GetMethod);
                    ctorGenerator.Emit(OpCodes.Stloc, valueLocal);
                    ctorGenerator.Emit(OpCodes.Ldarg_0);
                    ctorGenerator.Emit(OpCodes.Ldfld, managerField);
                    ctorGenerator.Emit(OpCodes.Ldstr, prop.Name);
                    ctorGenerator.Emit(OpCodes.Ldloc, valueLocal);
                    if (prop.PropertyType.IsValueType)
                    {
                        ctorGenerator.Emit(OpCodes.Box, prop.PropertyType);
                    }
                    ctorGenerator.Emit(OpCodes.Callvirt, setDefaultValueMethod);
                }
            ctorGenerator.Emit(OpCodes.Ret);

            /*** ctor(manager, capturedContext) ***/
            ConstructorBuilder ctor2Builder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                new Type[] { managerField.FieldType, capturedContextsField.FieldType });

            // TODO: Capturing a context in a runtime proxy probably creates memory leak
            // because the context can also have a reference to this proxy
            ILGenerator ctor2Generator = ctor2Builder.GetILGenerator();
            if (baseCtor != null)
            {
                ctor2Generator.Emit(OpCodes.Ldarg_0);
                ctor2Generator.Emit(OpCodes.Call, baseCtor);
            }
            ctor2Generator.Emit(OpCodes.Ldarg_0);
            ctor2Generator.Emit(OpCodes.Ldarg_1);
            ctor2Generator.Emit(OpCodes.Call, ctorBuilder);
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
    }
}
