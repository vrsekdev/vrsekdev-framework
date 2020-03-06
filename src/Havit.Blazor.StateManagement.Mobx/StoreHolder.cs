using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class StatePropertyChangedEventArgs
    {
        public string PropertyName { get; set; }
    }

    internal class CollectionItemsChangedEventArgs
    {
        public int OldCount { get; set; }

        public int NewCount { get; set; }

        public IEnumerable<object> ItemsAdded { get; set; }

        public IEnumerable<object> ItemsRemoved { get; set; }
    }

    internal class StoreHolder<TStore> : IStoreHolder<TStore>
        where TStore : class
    {
        public ObservableProperty RootObservableProperty { get; }

        public event EventHandler<StatePropertyChangedEventArgs> StatePropertyChangedEvent;

        public event EventHandler<CollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        public StoreHolder()
        {
            RootObservableProperty = new ObservableProperty(
                typeof(TStore),
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

        public ObservableProperty CreateObservableProperty(Type type)
        {
            return new ObservableProperty(
                type,
                new EventHandler<StatePropertyChangedEventArgs>(OnStatePropertyChanged),
                new EventHandler<CollectionItemsChangedEventArgs>(OnCollectionItemsChanged));
        }
    }
}
