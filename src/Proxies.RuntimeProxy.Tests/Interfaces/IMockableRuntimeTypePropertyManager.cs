using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy.Tests.Interfaces
{
    public interface IMockableRuntimeTypePropertyManager : IRuntimeProxyManager
    {
        object GetValue(string name);

        void SetValue(string name, object value);
    }
}
