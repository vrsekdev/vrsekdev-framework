using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy.Tests.Interfaces
{
    public interface IInterfaceWithDefaultProperty
    {
        public string StringProperty { get; set; }

        public string DefaultProperty => StringProperty;
    }
}
