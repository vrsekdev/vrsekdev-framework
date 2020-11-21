using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client;

namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Emit
{
    internal static class RuntimeProxyBuilder
    {
        private readonly static Lazy<ModuleBuilder> module;

        static RuntimeProxyBuilder()
        {
            module = new Lazy<ModuleBuilder>(() =>
            {
                var assemblyName = new AssemblyName("ContractRuntimeTypeAssembly");
                AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                return assemblyBuilder.DefineDynamicModule("MainModule");
            });
        }

        public static Type BuildRuntimeType(Type contractType)
        {
            return BuildRuntimeTypeInternal(contractType);
        }

        private static Type BuildRuntimeTypeInternal(Type contractType)
        {
            TypeBuilder typeBuilder = CreateTypeBuilder(contractType);

            FieldBuilder proxyField = AddProxyField(typeBuilder, contractType);

            var methods = GetPublicMethods(contractType).Where(x => x.IsAbstract);
            foreach (var method in methods)
            {
                AddMethod(typeBuilder, method, proxyField);
            }

            AddConstructor(contractType, typeBuilder, proxyField);

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

            return typeBuilder;
        }

        private static void AddConstructor(Type baseType, TypeBuilder typeBuilder, FieldBuilder proxyField)
        {
            ConstructorInfo baseCtor = baseType.GetConstructor(Type.EmptyTypes);

            /*** ctor(proxy) ***/
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                new Type[] { proxyField.FieldType });

            ILGenerator ctorGenerator = ctorBuilder.GetILGenerator();
            if (baseCtor != null)
            {
                ctorGenerator.Emit(OpCodes.Ldarg_0);
                ctorGenerator.Emit(OpCodes.Call, baseCtor);
            }
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Stfld, proxyField);
            ctorGenerator.Emit(OpCodes.Ret);
        }

        private static void AddMethod(TypeBuilder typeBuilder, MethodInfo interceptedMethod, FieldInfo proxyField)
        {
            MethodInfo interceptorMethod = proxyField.FieldType.GetMethod(nameof(RuntimeProxy<object>.InvokeRemoteMethod));

            Type returnType = interceptedMethod.ReturnType;
            ParameterInfo[] parameters = interceptedMethod.GetParameters().ToArray();

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                 interceptedMethod.Name,
                 MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot,
                 returnType,
                 parameters.Select(x => x.ParameterType).ToArray()); // same signature as base method

            ILGenerator methodGenerator = methodBuilder.GetILGenerator();
            LocalBuilder argsLocal = methodGenerator.DeclareLocal(typeof(object[]));
            methodGenerator.Emit(OpCodes.Ldarg_0);
            methodGenerator.Emit(OpCodes.Ldfld, proxyField);
            methodGenerator.Emit(OpCodes.Ldtoken, interceptedMethod);
            methodGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
            methodGenerator.Emit(OpCodes.Newarr, typeof(object));
            methodGenerator.Emit(OpCodes.Stloc, argsLocal);
            for (int i = 0; i < parameters.Length; i++)
            {
                methodGenerator.Emit(OpCodes.Ldloc, argsLocal);
                methodGenerator.Emit(OpCodes.Ldc_I4, i);
                methodGenerator.Emit(OpCodes.Ldarg, i + 1);
                if (parameters[i].ParameterType.IsValueType)
                {
                    methodGenerator.Emit(OpCodes.Box, parameters[i].ParameterType);
                }
                methodGenerator.Emit(OpCodes.Stelem_Ref);
            }
            methodGenerator.Emit(OpCodes.Ldloc, argsLocal);
            methodGenerator.Emit(OpCodes.Callvirt, interceptorMethod);
            methodGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, interceptedMethod);
        }

        private static FieldBuilder AddProxyField(TypeBuilder typeBuilder, Type contractType)
        {
            Type proxyType = typeof(RuntimeProxy<>).MakeGenericType(contractType);

            FieldBuilder proxyField = typeBuilder.DefineField(
                "proxy",
                proxyType,
                FieldAttributes.Private);

            return proxyField;
        }

        private static MethodInfo[] GetPublicMethods(Type type)
        {
            if (type.IsInterface)
            {
                List<MethodInfo> methodInfos = new List<MethodInfo>();

                List<Type> considered = new List<Type>();
                Queue<Type> queue = new Queue<Type>();
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

                    var typeMethods = subType.GetMethods(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newMethodInfos = typeMethods
                        .Where(x => !methodInfos.Contains(x));

                    methodInfos.InsertRange(0, newMethodInfos);
                }

                return methodInfos.ToArray();
            }

            return type.GetMethods(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
