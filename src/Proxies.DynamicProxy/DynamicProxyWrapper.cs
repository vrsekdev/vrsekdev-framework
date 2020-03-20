using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.DynamicProxy
{
    internal class DynamicProxyWrapper : IPropertyProxyWrapper
    {
        public T WrapPropertyObservable<T>(IPropertyProxy propertyProxy)
            where T : class
        {
            return DynamicPropertyProxy.Box<T>((DynamicPropertyProxy)propertyProxy);
        }

        public IPropertyProxy UnwrapPropertyObservable<T>(T propertyProxy)
            where T : class
        {
            return DynamicPropertyProxy.Unbox(propertyProxy);
        }
    }
}
