using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy
{
    internal interface IRuntimeProxyManager : IPropertyProxy
    {
        object Implementation { get; }
    }
}
