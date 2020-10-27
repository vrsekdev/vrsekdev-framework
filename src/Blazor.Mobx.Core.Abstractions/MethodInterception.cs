using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions
{
    public abstract class MethodInterception
    {
        public abstract MethodInfo GetInterceptedMethod();

        public abstract MethodInfo GetInterceptorMethod();

        public abstract object GetInterceptorTarget();

        public abstract bool ShouldProvideInterceptedTarget();

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is MethodInterception interception)) return false;

            return GetHashCode() == interception.GetHashCode();
        }

        public override int GetHashCode()
        {
            return GetInterceptedMethod().GetHashCode() ^ GetInterceptorMethod().GetHashCode();
        }
    }
}
