using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy
{
    internal class RuntimeProxyFactory : IPropertyProxyFactory
    {
        public IPropertyProxy Create(IObservableProperty observableProperty, bool readOnly = false)
        {
            return RuntimeProxyManagerHelper.CreateRuntimeManager(observableProperty, null, readOnly);
        }

        public IPropertyProxy Create(IObservableProperty observableProperty, MethodInterceptions interceptions, bool readOnly = false)
        {
            return RuntimeProxyManagerHelper.CreateRuntimeManager(observableProperty, interceptions, readOnly);
        }
    }
}
