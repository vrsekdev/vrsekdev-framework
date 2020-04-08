using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx
{
    internal abstract class ObserverBase<T>
        where T : class
    {
        private event EventHandler<PropertyAccessedEventArgs> PropertyAccessedEvent;

        protected Dictionary<IObservableProperty, IObservableContainer> observableContainers = new Dictionary<IObservableProperty, IObservableContainer>();

        public ObserverBase(
            IObservableHolder<T> observableHolder)
        {
            // no memory leak, because we dont store a reference to store holder
            observableHolder.CollectionItemsChangedEvent += StoreHolder_CollectionItemsChangedEvent;
            observableHolder.PropertyStateChangedEvent += StoreHolder_PropertyStateChangedEvent;
            PropertyAccessedEvent += OnPropertyAccessedEvent;
        }

        protected void OnPropertyAccessed(object sender, PropertyAccessedEventArgs e)
        {
            PropertyAccessedEvent?.Invoke(sender, e);
        }

        protected virtual void OnPropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            IObservableProperty observableProperty = e.PropertyProxy.ObservableProperty;
            if (!observableContainers.TryGetValue(observableProperty, out IObservableContainer container))
            {
                container = new ObservableContainer();
                observableContainers.Add(observableProperty, container);
            }

            container.OnPropertyAccessed(e.PropertyName);
        }

        private async void StoreHolder_PropertyStateChangedEvent(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            await TryInvokeAsync(e);
        }

        private async void StoreHolder_CollectionItemsChangedEvent(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            await TryInvokeAsync(e);
        }


        protected abstract ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedEventArgs e);

        protected abstract ValueTask<bool> TryInvokeAsync(ObservableCollectionItemsChangedEventArgs e);
    }
}
