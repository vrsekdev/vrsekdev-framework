﻿using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Components;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using VrsekDev.Blazor.Mobx.Abstractions.Utils;
using VrsekDev.Blazor.Mobx.Components;
using VrsekDev.Blazor.Mobx.Reactables.ComputedValues;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.StoreAccessors
{
    internal class StoreAccessor<TStore> : ObserverBase<TStore>, IStoreAccessor<TStore>
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
            IObservableProperty observableProperty = storeHolder.CreateObservableProperty(typeof(T));

            return CreateObservable<T>(observableProperty);
        }

        public T CreateObservable<T>(T instance) 
            where T : class
        {
            IObservableProperty observableProperty = storeHolder.CreateObservableProperty(typeof(T));
            observableProperty.OverwriteFrom(instance, false);

            return CreateObservable<T>(observableProperty);
        }

        internal T CreateObservable<T>(IObservableProperty observableProperty)
            where T : class
        {
            IPropertyProxy propertyProxy = propertyProxyFactory.Create(observableProperty);
            PlantSubscriber(propertyProxy);

            return propertyProxyWrapper.WrapPropertyObservable<T>(propertyProxy);
        }

        public void ExecuteInAction(Action action)
        {
            storeHolder.ExecuteInTransaction(action);
        }

        public Task ExecuteInActionAsync(Func<Task> action)
        {
            return storeHolder.ExecuteInTransactionAsync(action);
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
            if (consumer == null || !consumer.IsAlive())
            {
                return;
            }

            base.OnPropertyAccessedEvent(sender, e);
        }

        protected async override ValueTask<bool> TryInvokeAsync(ComputedValueChangedEventArgs e)
        {
            if (consumer == null)
            {
                return false;
            }

            if (!consumer.IsAlive())
            {
                return true;
            }

            await consumer.ForceUpdate();
            return true;
        }

        protected override async ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedEventArgs e)
        {
            if (consumer == null)
            {
                return false;
            }

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
            if (consumer == null)
            {
                return false;
            }

            if (!consumer.IsAlive())
            {
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
    }
}
