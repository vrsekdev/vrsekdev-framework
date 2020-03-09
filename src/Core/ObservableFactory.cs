using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObservableFactory : IObservableFactory
    {
        private readonly EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent;
        private readonly EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        public ObservableFactory(
            EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent,
            EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent)
        {
            this.statePropertyChangedEvent = statePropertyChangedEvent;
            this.collectionItemsChangedEvent = collectionItemsChangedEvent;
        }

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return new ObservableProperty(
                type,
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }

        public IObservableCollection<T> CreateObservableArray<T>()
        {
            return new ObservableCollection<T>(
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }
    }
}
