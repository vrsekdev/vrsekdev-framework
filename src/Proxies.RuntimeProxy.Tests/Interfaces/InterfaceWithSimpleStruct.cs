using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Interfaces
{
    public interface InterfaceWithSimpleStruct
    {
        SimpleStruct DefaultStruct { get; set; }
    }
}
