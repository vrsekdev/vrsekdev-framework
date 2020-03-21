using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IPropertyProxy : IObservableProxy
    {
        bool IsReadOnly { get; }

        IObservableProperty ObservableProperty { get; }
    }
}
