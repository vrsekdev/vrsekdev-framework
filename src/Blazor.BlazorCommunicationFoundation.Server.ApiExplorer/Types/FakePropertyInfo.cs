using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer.Types
{
    public class FakePropertyInfo : PropertyInfo
    {
        private readonly FakeType declaringType;
        private readonly ParameterInfo parameterInfo;
        private readonly FakePropertyGetMethodInfo getMethodInfo;

        public FakePropertyInfo(FakeType declaringType, ParameterInfo parameterInfo)
        {
            this.declaringType = declaringType;
            this.parameterInfo = parameterInfo;

            getMethodInfo = new FakePropertyGetMethodInfo(declaringType, this);
        }

        public override string Name => parameterInfo.Name;

        public override Type PropertyType => parameterInfo.ParameterType;

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override Type DeclaringType => declaringType;

        public override PropertyAttributes Attributes => throw new NotImplementedException();

        public override Type ReflectedType => throw new NotImplementedException();

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return getMethodInfo;
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return null;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return new Attribute[0];
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new Attribute[0];
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            return new MethodInfo[] { getMethodInfo };
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return new ParameterInfo[0];
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
