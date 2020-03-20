using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType
{
    public interface IRuntimeTypePropertyObservableManager : IPropertyObservable
    {
        object Implementation { get; }
    }
}
