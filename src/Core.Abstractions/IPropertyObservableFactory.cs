using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IPropertyObservableFactory
    {
        IPropertyObservable Create(IObservableProperty observableProperty);
    }
}
