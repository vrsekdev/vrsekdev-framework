﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Interfaces
{
    public interface IInterfaceWithNestedObservable
    {
        ISimpleInterface NestedObservable { get; set; }
    }
}
