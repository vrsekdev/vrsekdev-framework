using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy
{
    internal interface IRuntimeProxy
    {
        IRuntimeProxyManager Manager { get; }
    }
}
