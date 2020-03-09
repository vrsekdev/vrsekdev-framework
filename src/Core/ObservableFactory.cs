using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObservableFactory
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

        public ObservableProperty CreateObservableProperty(Type type)
        {
            return new ObservableProperty(
                type,
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }

        public ObservableArrayInternal CreateObservableArray<T>()
        {
            return new ObservableArrayInternal<T>(
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }
    }
}
