using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Interfaces
{
    public interface IMockableRuntimeTypePropertyManager : IRuntimeTypePropertyObservableManager
    {
        object GetValue(string name);

        void SetValue(string name, object value);
    }
}
