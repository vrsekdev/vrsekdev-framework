using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public class MethodInterceptorProxy
    {
        private readonly Action interceptor;

        public MethodInterceptorProxy(Action interceptor)
        {
            this.interceptor = interceptor;
        }

        public void Invoke()
        {
            interceptor();
        }
    }
}
