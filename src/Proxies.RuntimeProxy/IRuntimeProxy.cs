using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy
{
    public interface IRuntimeProxy
    {
        IRuntimeProxyManager Manager { get; }
    }
}
