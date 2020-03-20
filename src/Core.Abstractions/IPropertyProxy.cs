using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IPropertyProxy : IObservable<PropertyAccessedArgs>
    {
        IObservableProperty ObservableProperty { get; }
    }
}
