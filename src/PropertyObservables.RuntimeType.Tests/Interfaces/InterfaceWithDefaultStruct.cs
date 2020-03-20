using Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Interfaces
{
    public interface InterfaceWithDefaultStruct
    {
        DefaultStruct DefaultStruct { get; set; }
    }
}
