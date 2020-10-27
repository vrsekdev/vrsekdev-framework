using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public class StaticMethodInterception : MethodInterception
    {
        public override MethodInfo GetInterceptedMethod()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetInterceptorMethod()
        {
            throw new NotImplementedException();
        }

        public override object GetInterceptorTarget()
        {
            return null;
        }

        public override bool ShouldProvideInterceptedTarget()
        {
            throw new NotImplementedException();
        }
    }
}
