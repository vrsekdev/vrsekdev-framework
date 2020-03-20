using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy
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
            IRuntimeProxy impl = (IRuntimeProxy)propertyProxy;
            return impl.Manager;
        }
    }
}
