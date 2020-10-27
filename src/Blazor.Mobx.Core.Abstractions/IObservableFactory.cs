using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public interface IObservableFactory
    {
        IObservableProperty CreateObservableProperty(Type type);

        IObservableCollection<T> CreateObservableArray<T>(bool observeElement);
    }

    public interface IObservableFactoryFactory
    {
        IObservableFactory CreateFactory(
            EventHandler<ObservablePropertyStateChangedEventArgs> observablePropertyStateChangedEventArgs,
            EventHandler<ObservableCollectionItemsChangedEventArgs> observableCollectionItemsChangedEventArgs);
    }
}
