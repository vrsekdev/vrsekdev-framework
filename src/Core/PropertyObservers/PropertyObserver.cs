using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.StoreAccessors;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.PropertyObservers
{
    internal class PropertyObserver<T> : ObserverBase<T>
        where T : class
    {
        private readonly IObservableHolder<T> observableHolder;

        private IConsumerWrapper consumer;

        public T WrappedInstance { get; }

        public PropertyObserver(
            IObservableHolder<T> observableHolder,
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper) : base(observableHolder)
        {
            this.observableHolder = observableHolder;

            IPropertyProxy propertyProxy = propertyProxyFactory.Create(observableHolder.RootObservableProperty);
            WrappedInstance = propertyProxyWrapper.WrapPropertyObservable<T>(propertyProxy);

            PlantSubscriber(propertyProxy);
        }

        public void SetConsumer(IBlazorMobxComponent consumer)
        {
            Contract.Requires(consumer == null);

            this.consumer = new MobxConsumerWrapper(consumer);
        }

        public void InitializeValues(T instance)
        {
            observableHolder.RootObservableProperty.OverwriteFrom(instance, false);
        }

        private void PlantSubscriber(IPropertyProxy propertyProxy)
        {
            propertyProxy.Subscribe(new PropertyAccessedSubscriber(OnPropertyAccessed));
        }

        protected override void OnPropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            if (!consumer.IsAlive())
            {
                return;
            }

            base.OnPropertyAccessedEvent(sender, e);
        }

        protected override async ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedEventArgs e)
        {
            if (!consumer.IsAlive())
            {
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
            if (!consumer.IsAlive())
            {
                return true;
            }

            if (e.NewCount != e.OldCount || e.ItemsAdded.Any() || e.ItemsRemoved.Any())
            {
                await consumer.ForceUpdate();
                return true;
            }

            return false;
        }
    }
}
