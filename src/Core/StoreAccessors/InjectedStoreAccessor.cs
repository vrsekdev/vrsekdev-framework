using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Abstractions.Events;
using Havit.Blazor.StateManagement.Mobx.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.StoreAccessors
{
    internal class InjectedStoreAccessor<TStore> : IStoreAccessor<TStore>
        where TStore : class
    {
        private event EventHandler<PropertyAccessedEventArgs> PropertyAccessedEvent;

        private readonly IStoreHolder<TStore> storeHolder;
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        private HashSet<(IObservableProperty, string)> subscribedProperties = new HashSet<(IObservableProperty, string)>();

        private IConsumerWrapper consumer;

        public InjectedStoreAccessor(
            IStoreHolder<TStore> storeHolder,
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper)
        {
            if (!typeof(TStore).IsInterface)
            {
                throw new Exception("State type must be an interface");
            }
            this.storeHolder = storeHolder;
            this.propertyProxyFactory = propertyProxyFactory;
            this.propertyProxyWrapper = propertyProxyWrapper;

            IPropertyProxy propertyProxy = propertyProxyFactory.Create(storeHolder.RootObservableProperty);
            Store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);

            PropertyAccessedEvent += InjectedStoreAccessor_PropertyAccessedEvent;
            storeHolder.StatePropertyChangedEvent += StoreHolder_StatePropertyChangedEvent;
            storeHolder.CollectionItemsChangedEvent += StoreHolder_CollectionItemsChangedEvent;

            PlantSubscriber(propertyProxy);
        }

        public TStore Store { get; }

        public void SetConsumer(BlazorMobxComponentBase consumer)
        {
            Contract.Requires(consumer == null);

            this.consumer = new MobxConsumerWrapper(consumer);
        }

        public void SetConsumer(ComponentBase consumer)
        {
            Contract.Requires(consumer == null);
            if (consumer is BlazorMobxComponentBase<TStore> mobxConsumer)
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

            subscribedProperties = null;
            consumer = null;
        }

        private void PlantSubscriber(IPropertyProxy propertyProxy)
        {
            propertyProxy.Subscribe(new PropertyAccessedSubscriber(PropertyAccessedEvent));
        }

        private void InjectedStoreAccessor_PropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            IObservableProperty observableProperty = e.PropertyProxy.ObservableProperty;
            var key = (observableProperty, e.PropertyName);
            if (!subscribedProperties.Contains(key))
            {
                subscribedProperties.Add(key);
            }
        }

        private async void StoreHolder_StatePropertyChangedEvent(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            IObservableProperty observableProperty = (IObservableProperty)sender;
            string propertyName = e.PropertyName;

            if (subscribedProperties.Contains((observableProperty, propertyName)))
            {
                if (consumer.IsRendered())
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

        private interface IConsumerWrapper
        {
            Task ForceUpdate();

            bool IsAlive();

            bool IsRendered();
        }

        private class MobxConsumerWrapper : IConsumerWrapper
        {
            private readonly WeakReference<BlazorMobxComponentBase> consumerReference;
            private readonly string componentName;

            public MobxConsumerWrapper(BlazorMobxComponentBase consumer)
            {
                this.consumerReference = new WeakReference<BlazorMobxComponentBase>(consumer);
                this.componentName = consumer.GetType().Name;
            }

            public Task ForceUpdate()
            {
                if (!consumerReference.TryGetTarget(out BlazorMobxComponentBase consumer))
                {
#if DEBUG
                    throw new Exception("Component is dead");
#else
                    return Task.CompletedTask;
#endif
                }

                return consumer.ForceUpdate();
            }

            public bool IsAlive()
            {
                return consumerReference.TryGetTarget(out _);
            }

            public bool IsRendered()
            {
                if (!consumerReference.TryGetTarget(out BlazorMobxComponentBase consumer))
                {
#if DEBUG
                    throw new Exception("Component is dead");
#else
                    return Task.CompletedTask;
#endif
                }

                return consumer.IsRendered();
            }

            public override string ToString()
            {
                return componentName;
            }
        }

        private class ReflectionConsumerWrapper : IConsumerWrapper
        {
            #region static
            private static readonly Func<ComponentBase, Action, Task> ComponentBaseInvokeAsync;
            private static readonly Action<ComponentBase> ComponentBaseStateHasChanged;
            
            static ReflectionConsumerWrapper()
            {
                MethodInfo invokeAsyncMethodInfo =
				typeof(ComponentBase).GetMethod(
					name: "InvokeAsync",
					bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
					binder: null,
					types: new[] { typeof(Action) },
					modifiers: null);
                ComponentBaseInvokeAsync = (Func<ComponentBase, Action, Task>)
                    Delegate.CreateDelegate(typeof(Func<ComponentBase, Action, Task>), invokeAsyncMethodInfo);

                MethodInfo stateHasChangedMethodInfo =
                    typeof(ComponentBase).GetMethod(
                        name: "StateHasChanged",
                        bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

                ComponentBaseStateHasChanged = (Action<ComponentBase>)Delegate.CreateDelegate(typeof(Action<ComponentBase>), stateHasChangedMethodInfo);
            }
            #endregion

            private readonly WeakReference<ComponentBase> consumerReference;

            public ReflectionConsumerWrapper(ComponentBase consumer)
            {
                this.consumerReference = new WeakReference<ComponentBase>(consumer);
            }

            public Task ForceUpdate()
            {
                if (!consumerReference.TryGetTarget(out ComponentBase consumer))
                {
#if DEBUG
                    throw new Exception("Component is dead");
#else
                    return Task.CompletedTask;
#endif
                }

                return ComponentBaseInvokeAsync(consumer, () => ComponentBaseStateHasChanged(consumer));
            }

            public bool IsRendered()
            {
                return true;
            }

            public bool IsAlive()
            {
                return consumerReference.TryGetTarget(out _);
            }
        }
    }
}
