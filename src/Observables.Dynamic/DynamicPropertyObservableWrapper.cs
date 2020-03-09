using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Observables.Dynamic
{
    internal class DynamicPropertyObservableWrapper : IPropertyObservableWrapper
    {
        public T WrapPropertyObservable<T>(IPropertyObservable propertyObservable)
            where T : class
        {
            return DynamicPropertyObservable.Box<T>((DynamicPropertyObservable)propertyObservable);
        }

        public IPropertyObservable UnwrapPropertyObservable<T>(T observableValue)
        {
            return DynamicPropertyObservable.Unbox(observableValue);
        }
    }
}
