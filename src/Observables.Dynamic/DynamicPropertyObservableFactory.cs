using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Observables.Dynamic
{
    internal class DynamicPropertyObservableFactory : IPropertyObservableFactory
    {
        public IPropertyObservable Create(IObservableProperty observableProperty)
        {
            return DynamicPropertyObservable.Create(observableProperty);
        }
    }
}
