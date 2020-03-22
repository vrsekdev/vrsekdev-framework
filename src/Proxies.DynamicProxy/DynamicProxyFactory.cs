using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.DynamicProxy
{
    internal class DynamicProxyFactory : IPropertyProxyFactory
    {
        public IPropertyProxy Create(IObservableProperty observableProperty, bool readOnly = false)
        {
            return DynamicPropertyProxy.Create(observableProperty, readOnly);
        }
    }
}
