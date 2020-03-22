using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Observables.Default
{
    internal class ObservableFactory : IObservableFactory
    {
        private readonly EventHandler<ObservablePropertyStateChangedEventArgs> observablePropertyStateChangedEventArgs;
        private readonly EventHandler<ObservableCollectionItemsChangedEventArgs> observableCollectionItemsChangedEventArgs;

        public ObservableFactory(
            EventHandler<ObservablePropertyStateChangedEventArgs> observablePropertyStateChangedEventArgs,
            EventHandler<ObservableCollectionItemsChangedEventArgs> observableCollectionItemsChangedEventArgs)
        {
            this.observablePropertyStateChangedEventArgs = observablePropertyStateChangedEventArgs;
            this.observableCollectionItemsChangedEventArgs = observableCollectionItemsChangedEventArgs;
        }

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return new ObservableProperty(
                type,
                observablePropertyStateChangedEventArgs,
                observableCollectionItemsChangedEventArgs);
        }

        public IObservableCollection<T> CreateObservableArray<T>(bool observeElement)
        {
            return new ObservableCollection<T>(
                observeElement,
                observablePropertyStateChangedEventArgs,
                observableCollectionItemsChangedEventArgs);
        }
    }
}
