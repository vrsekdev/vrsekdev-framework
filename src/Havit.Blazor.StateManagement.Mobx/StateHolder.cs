﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    public class StatePropertyChangedEventArgs
    {
        public string PropertyName { get; set; }
    }

    public class CollectionItemsChangedEventArgs
    {
        public CollectionItemsChangedEventArgs(
            IEnumerable<object> itemsAdded,
            IEnumerable<object> itemsRemoved)
        {
            ItemsAdded = itemsAdded;
            ItemsRemoved = itemsRemoved;
        }

        public int OldCount { get; set; }

        public int NewCount { get; set; }

        public IEnumerable<object> ItemsAdded { get; }

        public IEnumerable<object> ItemsRemoved { get; }
    }

    public class StateHolder<TState> : IStateHolder<TState>
        where TState : class
    {
        public ObservableProperty RootObservableProperty { get; }

        public event EventHandler<StatePropertyChangedEventArgs> StatePropertyChangedEvent;

        public event EventHandler<CollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        public StateHolder()
        {
            RootObservableProperty = new ObservableProperty(
                typeof(TState),
                new EventHandler<StatePropertyChangedEventArgs>(OnStatePropertyChanged),
                new EventHandler<CollectionItemsChangedEventArgs>(OnCollectionItemsChanged));
        }

        private void OnStatePropertyChanged(object sender, StatePropertyChangedEventArgs e)
        {
            StatePropertyChangedEvent?.Invoke(sender, e);
        }

        private void OnCollectionItemsChanged(object sender, CollectionItemsChangedEventArgs e)
        {
            CollectionItemsChangedEvent?.Invoke(sender, e);
        }
    }
}
