﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Interfaces
{
    public interface IInterfaceWithNestedObservable
    {
        IClassicInterface NestedObservable { get; set; }
    }
}
