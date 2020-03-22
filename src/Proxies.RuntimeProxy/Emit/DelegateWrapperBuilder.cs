using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Emit
{
    internal class DelegateWrapperBuilder
    {
        public static Delegate CreateDelegateHelper(Type delegateType, object target, RuntimeMethodHandle methodHandle)
        {
            var methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(methodHandle);
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }

        private readonly TypeBuilder typeBuilder;
        private readonly Type baseType;

        private TypeBuilder nestedType;
        private FieldBuilder delegateTypeField;
        private FieldBuilder targetField;
        private FieldBuilder baseMethodInfoField;
        private FieldBuilder delegateField;

        public DelegateWrapperBuilder(
            TypeBuilder typeBuilder, 
            Type baseType)
        {
            this.typeBuilder = typeBuilder;
            this.baseType = baseType;
        }

        public DelegateWrapperTypeContainer AddDelegateWrapperType()
        {
            nestedType = typeBuilder.DefineNestedType("DelegateWrapper", TypeAttributes.NestedPublic | TypeAttributes.Class);

            delegateTypeField = nestedType.DefineField("delegateType", typeof(Type), FieldAttributes.Private);
            targetField = nestedType.DefineField("target", typeof(object), FieldAttributes.Private);
            baseMethodInfoField = nestedType.DefineField("methodInfo", typeof(MethodInfo), FieldAttributes.Private);
            delegateField = nestedType.DefineField("createdDelegate", typeof(Delegate), FieldAttributes.Public);

            MethodBuilder createDelegateMethod = AddCreateDelegateMethod();
            ConstructorBuilder ctorBuilder = AddConstructor();

            return new DelegateWrapperTypeContainer
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

        private ConstructorBuilder AddConstructor()
        {
            var getMethodInfoFromHandle = typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle) });
            var getTypeFromHandle = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), new[] { typeof(RuntimeTypeHandle) });

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

            return ctorBuilder;
        }

        private MethodBuilder AddCreateDelegateMethod()
        {
            MethodInfo createDelegateHelperMethod = typeof(DelegateWrapperBuilder).GetMethod(nameof(DelegateWrapperBuilder.CreateDelegateHelper));

            MethodBuilder createDelegateMethod = nestedType.DefineMethod(
                "CreateDelegate_" + Guid.NewGuid(),
                MethodAttributes.Public,
                typeof(Delegate),
                new Type[] { typeof(RuntimeMethodHandle) });

            ILGenerator createDelegateGenerator = createDelegateMethod.GetILGenerator();
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

            return createDelegateMethod;
        }
    }

    internal class DelegateWrapperTypeContainer
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
