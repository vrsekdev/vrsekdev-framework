using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy
{
    public interface IRuntimeProxyManager : IPropertyProxy
    {
        object Implementation { get; }
    }
}
