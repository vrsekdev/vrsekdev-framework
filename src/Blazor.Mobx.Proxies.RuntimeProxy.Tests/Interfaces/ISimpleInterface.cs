using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Interfaces
{
    public interface ISimpleInterface
    {
        SimpleClass ReferenceTypeValue { get; set; }

        string StringValue { get; set; }

        int ValueTypeValue { get; set; }
    }
}
