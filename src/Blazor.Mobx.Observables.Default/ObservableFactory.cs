using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Observables.Default
{
    internal class ObservableFactory : IObservableFactory
    {
        private readonly EventHandler<ObservablePropertyStateChangedArgs> observablePropertyStateChangedEventArgs;
        private readonly EventHandler<ObservableCollectionItemsChangedArgs> observableCollectionItemsChangedEventArgs;

        public ObservableFactory(
            EventHandler<ObservablePropertyStateChangedArgs> observablePropertyStateChangedEventArgs,
            EventHandler<ObservableCollectionItemsChangedArgs> observableCollectionItemsChangedEventArgs)
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
