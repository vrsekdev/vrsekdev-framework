using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.Components;
using Havit.Blazor.Mobx.Reactables.ComputedValues;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.StoreAccessors
{
    internal class StoreAccessor<TStore> : IStoreAccessor<TStore>
        where TStore : class
    {
        private event EventHandler<PropertyAccessedEventArgs> PropertyAccessedEvent;

        private Dictionary<IObservableProperty, IObservableContainer> observableContainers = new Dictionary<IObservableProperty, IObservableContainer>();

        private readonly IStoreHolder<TStore> storeHolder;
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        private IConsumerWrapper consumer;

        public StoreAccessor(
            IStoreHolder<TStore> storeHolder,
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper)
        {
            this.storeHolder = storeHolder;
            this.propertyProxyFactory = propertyProxyFactory;
            this.propertyProxyWrapper = propertyProxyWrapper;

            IPropertyProxy propertyProxy = propertyProxyFactory.Create(storeHolder.RootObservableProperty, storeHolder.StoreReactables);
            Store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);

            PropertyAccessedEvent += InjectedStoreAccessor_PropertyAccessedEvent;
            storeHolder.StatePropertyChangedEvent += StoreHolder_StatePropertyChangedEvent;
            storeHolder.CollectionItemsChangedEvent += StoreHolder_CollectionItemsChangedEvent;

            PlantSubscriber(propertyProxy);
        }

        public TStore Store { get; }

        public void SetConsumer(IBlazorMobxComponent consumer)
        {
            Contract.Requires(consumer == null);

            this.consumer = new MobxConsumerWrapper(consumer);
        }

        public void SetConsumer(ComponentBase consumer)
        {
            Contract.Requires(consumer == null);
            if (consumer is IBlazorMobxComponent mobxConsumer)
            {
                SetConsumer(mobxConsumer);
                return;
            }

            this.consumer = new ReflectionConsumerWrapper(consumer);
        }

        public T CreateObservable<T>()
             where T : class
        {
            // TODO: Create observable array method
            IObservableProperty observableProperty = storeHolder.CreateObservableProperty(typeof(T));

            return CreateObservable<T>(observableProperty);
        }

        internal T CreateObservable<T>(IObservableProperty observableProperty)
            where T : class
        {
            IPropertyProxy propertyProxy = propertyProxyFactory.Create(observableProperty);
            return propertyProxyWrapper.WrapPropertyObservable<T>(propertyProxy);
        }

        public void ResetStore()
        {
            propertyProxyWrapper.UnwrapPropertyObservable(Store).ObservableProperty.ResetValues();
        }

        public void Dispose()
        {
            PropertyAccessedEvent -= InjectedStoreAccessor_PropertyAccessedEvent;
            storeHolder.StatePropertyChangedEvent -= StoreHolder_StatePropertyChangedEvent;
            storeHolder.CollectionItemsChangedEvent -= StoreHolder_CollectionItemsChangedEvent;

            consumer = null;
            observableContainers = null;
        }

        private void PlantSubscriber(IPropertyProxy propertyProxy)
        {
            propertyProxy.Subscribe(new PropertyAccessedSubscriber(PropertyAccessedEvent));
        }

        private void InjectedStoreAccessor_PropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            if (!consumer.IsAlive())
            {
                Dispose();
                return;
            }

            IObservableProperty observableProperty = e.PropertyProxy.ObservableProperty;
            if (!observableContainers.TryGetValue(observableProperty, out IObservableContainer container))
            {
                container = new ObservableContainer();
                observableContainers.Add(observableProperty, container);
            }

            container.OnPropertyAccessed(e.PropertyName);
        }

        private async void StoreHolder_StatePropertyChangedEvent(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            if (!consumer.IsAlive())
            {
                Dispose();
                return;
            }

            if (!consumer.IsRendered())
                return;

            IObservableProperty observableProperty = (IObservableProperty)sender;
            string propertyName = e.PropertyName;
            if (observableContainers.TryGetValue(observableProperty, out IObservableContainer container))
            {
                if (container.IsSubscribed(propertyName))
                {
                    await consumer.ForceUpdate();
                }
            }
        }

        private async void StoreHolder_CollectionItemsChangedEvent(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            if (e.NewCount != e.OldCount || e.ItemsAdded.Any() || e.ItemsRemoved.Any())
            {
                if (consumer.IsRendered())
                {
                    await consumer.ForceUpdate();
                }
            }
        }
    }
}
