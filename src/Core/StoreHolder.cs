using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class StoreHolder<TStore> : IStoreHolder<TStore>
        where TStore : class
    {
        private readonly IObservableFactory observableFactory;

        public IObservableProperty RootObservableProperty { get; }

        public event EventHandler<ObservablePropertyStateChangedEventArgs> StatePropertyChangedEvent;
        public event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        public StoreHolder(
            IObservableFactoryFactory observableFactoryFactory)
        {
            this.observableFactory = observableFactoryFactory.Create(
                OnStatePropertyChanged,
                OnCollectionItemsChanged);
            RootObservableProperty = CreateObservableProperty(typeof(TStore));
        }

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return observableFactory.CreateObservableProperty(type);
        }

        private void OnStatePropertyChanged(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            StatePropertyChangedEvent?.Invoke(sender, e);
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            CollectionItemsChangedEvent?.Invoke(sender, e);
        }
    }
}
