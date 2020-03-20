using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType
{
    public interface IRuntimeTypeImpl
    {
        IRuntimeTypePropertyObservableManager Manager { get; }
    }
}
