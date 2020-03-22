using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy
{
    public interface IRuntimeProxyManager : IPropertyProxy
    {
        object Implementation { get; }
    }
}
