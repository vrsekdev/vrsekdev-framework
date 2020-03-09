using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IPropertyObservableWrapper
    {
        T WrapPropertyObservable<T>(IPropertyObservable propertyObservable)
            where T : class;

        IPropertyObservable UnwrapPropertyObservable<T>(T observableValue);
    }
}
