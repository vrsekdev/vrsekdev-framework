using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Classes
{
    public class ClassWithVirtualMethod
    {
        public bool InterceptedMethodCalled { get; private set; }

        public virtual void MethodToIntercept()
        {
            InterceptedMethodCalled = true;
        }
    }
}
