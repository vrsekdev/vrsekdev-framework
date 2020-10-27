using VrsekDev.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Proxies.DynamicProxy
{
    internal class DynamicProxyFactory : IPropertyProxyFactory
    {
        public IPropertyProxy Create(IObservableProperty observableProperty, bool readOnly = false)
        {
            return DynamicPropertyProxy.Create(observableProperty, readOnly);
        }

        public IPropertyProxy Create(IObservableProperty observableProperty, MethodInterceptions interceptions, bool readOnly = false)
        {
            throw new NotImplementedException();
        }
    }
}
