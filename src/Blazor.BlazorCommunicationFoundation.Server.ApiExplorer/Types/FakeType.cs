using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer.Types
{
    public class FakeType : Type
    {
        private const string TypeNamespace = "VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer.Types";

        private readonly MethodInfo methodInfo;
        private readonly PropertyInfo[] properties;

        public FakeType(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;

            properties = methodInfo.GetParameters().Select(x => new FakePropertyInfo(this, x)).ToArray<PropertyInfo>();
        }

        public override string Name => methodInfo.Name + "Parameters";

        public override string Namespace => TypeNamespace;

        public override string FullName => Namespace + "." + Name;

        public override Type BaseType => null;

        public override string AssemblyQualifiedName => "FakeParameterType, " + FullName;

        public override Type UnderlyingSystemType => this;

        public override Guid GUID => throw new NotImplementedException();

        public override Assembly Assembly => throw new NotImplementedException();

        public override Module Module => throw new NotImplementedException();

        public override bool IsGenericType => false;

        public override bool IsGenericTypeDefinition => false;

        public override bool IsConstructedGenericType => false;

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return properties.SingleOrDefault(x => x.Name == name);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return properties;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return new Attribute[0];
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new Attribute[0];
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            return null;
        }

        public override Type[] GetInterfaces()
        {
            return Type.EmptyTypes;
        }

        public override Type GetElementType()
        {
            return null;
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return TypeAttributes.Public | TypeAttributes.Class;
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return new ConstructorInfo[0];
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return null;
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return new FieldInfo[0];
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return properties;
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return null;
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return new MethodInfo[0];
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return null;
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return new Type[0];
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }

        protected override bool HasElementTypeImpl()
        {
            return false;
        }

        protected override bool IsArrayImpl()
        {
            return false;
        }

        protected override bool IsByRefImpl()
        {
            return false;
        }

        protected override bool IsPrimitiveImpl()
        {
            return false;
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }
    }
}
