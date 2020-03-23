using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables
{
    internal class ReactableInvoker<TStore>
        where TStore : class
    {
        private event EventHandler<PropertyAccessedEventArgs> PropertyAccessedEvent;

        private readonly IInvokableReactable reactable;

        private Dictionary<IObservableProperty, IObservableContainer> observableContainers = new Dictionary<IObservableProperty, IObservableContainer>();

        public ReactableInvoker(
            IInvokableReactable reactable,
            IStoreHolder<TStore> storeHolder)
        {
            this.reactable = reactable;

            // no memory leak, because we dont store a reference to store holder
            storeHolder.CollectionItemsChangedEvent += StoreHolder_CollectionItemsChangedEvent;
            storeHolder.StatePropertyChangedEvent += StoreHolder_StatePropertyChangedEvent;
            PropertyAccessedEvent += ReactableInvoker_PropertyAccessedEvent;
        }

        public void PlantSubscriber(IPropertyProxy propertyProxy)
        {
            propertyProxy.Subscribe(new PropertyAccessedSubscriber(PropertyAccessedEvent));
        }

        private void ReactableInvoker_PropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            IObservableProperty observableProperty = e.PropertyProxy.ObservableProperty;
            if (!observableContainers.TryGetValue(observableProperty, out IObservableContainer container))
            {
                container = new ObservableContainer();
                observableContainers.Add(observableProperty, container);
            }

            container.OnPropertyAccessed(e.PropertyName);
        }

        private void StoreHolder_StatePropertyChangedEvent(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            IObservableProperty observableProperty = (IObservableProperty)sender;
            string propertyName = e.PropertyName;
            if (observableContainers.TryGetValue(observableProperty, out IObservableContainer container))
            {
                if (container.IsSubscribed(propertyName))
                {
                    reactable.Invoke();
                }
            }
        }

        private void StoreHolder_CollectionItemsChangedEvent(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            if (e.NewCount != e.OldCount || e.ItemsAdded.Any() || e.ItemsRemoved.Any())
            {
                reactable.Invoke();
            }
        }
    }
}
