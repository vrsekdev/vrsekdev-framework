//#define ENABLE_CACHING

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy
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

        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static Dictionary<Type, Type> runtimeTypeCache = new Dictionary<Type, Type>();

        public static Type BuildRuntimeType(Type interfaceType, MethodInfo getMethod, MethodInfo setMethod)
        {
#if !ENABLE_CACHING
            return BuildRuntimeTypeInternal(interfaceType, getMethod, setMethod);
#else
            cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (runtimeTypeCache.TryGetValue(interfaceType, out Type runtimeType))
                {
                    return runtimeType;
                }

                cacheLock.EnterWriteLock();
                try
                {
                    runtimeType = BuildRuntimeTypeInternal(interfaceType, getMethod, setMethod);
                    runtimeTypeCache.Add(interfaceType, runtimeType);

                    return runtimeType;
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
#endif
        }

        private static Type BuildRuntimeTypeInternal(Type interfaceType, MethodInfo getMethod, MethodInfo setMethod)
        {
            TypeBuilder typeBuilder = CreateTypeBuilder(interfaceType);
            FieldBuilder managerField = AddManagerProperty(typeBuilder);

            AddConstructor(typeBuilder, managerField);
            foreach (var propertyInfo in GetPublicProperties(interfaceType).Where(x => x.GetMethod.IsAbstract))
            {
                AddProperty(typeBuilder, propertyInfo, managerField, getMethod, setMethod);
            }

            return typeBuilder.CreateType();
        }

        private static TypeBuilder CreateTypeBuilder(Type interfaceType)
        {
            var typeSignature = interfaceType.Name + "_RuntimeImpl_" + Guid.NewGuid();
            TypeBuilder typeBuilder = module.Value.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class,
                    null);
            typeBuilder.AddInterfaceImplementation(interfaceType);
            typeBuilder.AddInterfaceImplementation(typeof(IRuntimeProxy));

            return typeBuilder;
        }

        private static void AddConstructor(TypeBuilder typeBuilder, FieldBuilder managerField)
        {
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName, 
                CallingConventions.Standard,
                new Type[] { managerField.FieldType });

            ILGenerator ctorGenerator = ctorBuilder.GetILGenerator();
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Stfld, managerField);
            ctorGenerator.Emit(OpCodes.Ret);
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

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
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
            propertyBuilder.SetGetMethod(propGetAccessor);
            propertyBuilder.SetSetMethod(propSetAccessor);
        }

        private static PropertyInfo[] GetPublicProperties(Type type)
        {
            Contract.Requires(type.IsInterface);

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
                    if (considered.Contains(subInterface))
                    {
                        continue;
                    }

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
    }
}
