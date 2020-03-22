using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public struct MethodInterception
    {
        public MethodInfo InterceptedMethod { get; set; }

        public MethodInfo Interceptor { get; set; }
    }
}
