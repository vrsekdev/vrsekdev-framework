using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Interfaces
{
    public interface IClassicInterface
    {
        string ReferenceType { get; set; }

        int ValueType { get; set; }
    }
}
