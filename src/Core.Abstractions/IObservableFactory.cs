using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IObservableFactory
    {
        IObservableProperty CreateObservableProperty(Type type);

        IObservableCollection<T> CreateObservableArray<T>();
    }
}
