using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.Abstractions.Utils;
using Havit.Blazor.Mobx.Components;
using Havit.Blazor.Mobx.Reactables.ComputedValues;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.StoreAccessors
{
    internal class StoreAccessor<TStore> : StoreObserverBase<TStore>, IStoreAccessor<TStore>
        where TStore : class
    {
        private readonly IStoreHolder<TStore> storeHolder;
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        private IConsumerWrapper consumer;

        public StoreAccessor(
            IStoreHolder<TStore> storeHolder,
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper) : base(storeHolder)
        {
            this.storeHolder = storeHolder;
            this.propertyProxyFactory = propertyProxyFactory;
            this.propertyProxyWrapper = propertyProxyWrapper;

            IPropertyProxy propertyProxy = propertyProxyFactory.Create(storeHolder.RootObservableProperty, storeHolder.StoreReactables);
            Store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);
            storeHolder.DependencyInjector.InjectDependency(Store);

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

        private void PlantSubscriber(IPropertyProxy propertyProxy)
        {
            propertyProxy.Subscribe(new PropertyAccessedSubscriber(OnPropertyAccessed));
        }

        protected override void OnPropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            if (consumer.IsAlive())
            {
                Dispose();
                return;
            }

            base.OnPropertyAccessedEvent(sender, e);
        }

        protected async override ValueTask<bool> TryInvokeAsync(ComputedValueChangedEventArgs e)
        {
            if (consumer.IsAlive())
            {
                Dispose();
                return true;
            }

            await consumer.ForceUpdate();
            return true;
        }

        protected override async ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedEventArgs e)
        {
            if (consumer.IsAlive())
            {
                Dispose();
                return true;
            }

            IObservableProperty observableProperty = e.ObservableProperty;
            string propertyName = e.PropertyInfo.Name;
            if (observableContainers.TryGetValue(observableProperty, out IObservableContainer container))
            {
                if (container.IsSubscribed(propertyName))
                {
                    await consumer.ForceUpdate();
                    return true;
                }
            }

            return false;
        }

        protected override async ValueTask<bool> TryInvokeAsync(ObservableCollectionItemsChangedEventArgs e)
        {
            if (consumer.IsAlive())
            {
                Dispose();
                return true;
            }

            if (e.ObservableCollection.ElementType == typeof(Task))
            {
                foreach (var task in e.ItemsAdded.Cast<Task>())
                {
                    _ = task.ContinueWith(t => { 
                        consumer?.ForceUpdate(); 
                    });
                }
                return true;
            }

            if (e.NewCount != e.OldCount || e.ItemsAdded.Any() || e.ItemsRemoved.Any())
            {
                await consumer.ForceUpdate();
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            consumer = null;
        }
    }
}
