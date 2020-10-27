using VrsekDev.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy
{
    internal class RuntimeProxyWrapper : IPropertyProxyWrapper
    {
        public T WrapPropertyObservable<T>(IPropertyProxy propertyProxy)
            where T : class
        {
            return ((RuntimeProxyManager<T>)propertyProxy).Implementation;
        }

        public IPropertyProxy UnwrapPropertyObservable<T>(T propertyProxy)
            where T : class
        {
            IRuntimeProxy impl = propertyProxy as IRuntimeProxy;
            return impl?.Manager;
        }
    }
}
