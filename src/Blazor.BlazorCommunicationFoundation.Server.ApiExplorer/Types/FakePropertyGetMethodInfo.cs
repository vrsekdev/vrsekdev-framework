using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer.Types
{
    public class FakePropertyGetMethodInfo : MethodInfo
    {
        private readonly FakeType declaringType;
        private readonly FakePropertyInfo fakePropertyInfo;

        public FakePropertyGetMethodInfo(FakeType declaringType, FakePropertyInfo fakePropertyInfo)
        {
            this.declaringType = declaringType;
            this.fakePropertyInfo = fakePropertyInfo;
        }

        public override string Name => "get_" + fakePropertyInfo.Name;

        public override Type DeclaringType => declaringType;

        public override MethodAttributes Attributes => MethodAttributes.Public;

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => fakePropertyInfo;

        public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new Attribute[0];
        }

        public override Type ReflectedType => throw new NotImplementedException();

        public override MethodInfo GetBaseDefinition()
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }


        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
}
