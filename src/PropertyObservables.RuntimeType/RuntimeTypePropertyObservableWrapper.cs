using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType
{
    internal class RuntimeTypePropertyObservableWrapper : IPropertyObservableWrapper
    {
        public T WrapPropertyObservable<T>(IPropertyObservable propertyObservable)
            where T : class
        {
            return ((RuntimeTypePropertyObservableManager<T>)propertyObservable).Implementation;
        }

        public IPropertyObservable UnwrapPropertyObservable<T>(T observableValue)
            where T : class
        {
            IRuntimeTypeImpl impl = (IRuntimeTypeImpl)observableValue;
            return impl.Manager;
        }
    }
}
