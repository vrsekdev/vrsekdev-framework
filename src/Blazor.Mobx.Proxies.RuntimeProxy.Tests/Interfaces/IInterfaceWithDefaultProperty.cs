using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Interfaces
{
    public interface IInterfaceWithDefaultProperty
    {
        public string StringProperty { get; set; }

        public string DefaultProperty => StringProperty;
    }
}
