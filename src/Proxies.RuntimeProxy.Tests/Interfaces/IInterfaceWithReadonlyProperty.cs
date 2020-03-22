using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Interfaces
{
    public interface IInterfaceWithReadonlyProperty
    {
        public string ReadonlyStringProperty { get; }
    }
}
