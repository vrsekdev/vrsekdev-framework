using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions
{
    public class ClassMethodInterception : MethodInterception
    {
        public MethodInfo InterceptedMethod { get; set; }

        public MethodInfo InterceptorMethod { get; set; }

        public object InterceptorTarget { get; set; }

        public bool ProvideInterceptedTarget { get; set; } = true;

        public override MethodInfo GetInterceptedMethod()
        {
            return InterceptedMethod;
        }

        public override MethodInfo GetInterceptorMethod()
        {
            return InterceptorMethod;
        }

        public override object GetInterceptorTarget()
        {
            return InterceptorTarget;
        }

        public override bool ShouldProvideInterceptedTarget()
        {
            return ProvideInterceptedTarget;
        }
    }
}
