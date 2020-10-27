using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Classes
{
    public class ClassWithVirtualMethod
    {
        public string ActionParameter { get; private set; }

        public bool InterceptedMethodCalled { get; private set; }
        public bool InterceptedMethod2Called { get; private set; }

        public virtual void ActionToIntercept()
        {
            InterceptedMethodCalled = true;
        }

        public virtual void ActionToIntercept2()
        {
            InterceptedMethod2Called = true;
        }

        public virtual string FunctionToIntercept()
        {
            return "base";
        }

        public virtual string FunctionWithParameterToIntercept(string param)
        {
            return param;
        }

        public virtual ClassWithVirtualMethod FunctionReturningThis()
        {
            return this;
        }

        public virtual void ActionWithParameter(string parameter)
        {
            ActionParameter = parameter;
        }
    }
}
