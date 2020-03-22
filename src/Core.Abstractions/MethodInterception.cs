using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public abstract class MethodInterception
    {
        public abstract MethodInfo GetInterceptedMethod();

        public abstract MethodInfo GetInterceptorMethod();

        public abstract object GetInterceptorTarget();

        public abstract bool ShouldProvideInterceptedTarget();
    }
}
