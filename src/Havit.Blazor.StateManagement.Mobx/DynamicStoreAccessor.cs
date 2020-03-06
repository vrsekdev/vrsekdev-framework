using Havit.Blazor.StateManagement.Mobx.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class PropertyAccessedEventArgs
    {
        public Type ObservedType { get; set; }

        public string PropertyName { get; set; }
    }

    internal class DynamicStoreAccessor<TStore> : IStoreAccessor<TStore>, IDisposable
        where TStore : class
    {
        private event EventHandler<PropertyAccessedEventArgs> PropertyAccessedEvent;

        private readonly IStoreHolder<TStore> storeHolder;

        private IDisposable rootObserverDisposer;
        private HashSet<(Type, string)> subscribedProperties = new HashSet<(Type, string)>();
        private Dictionary<object, IDisposable> collectionItemObserverDisposables = new Dictionary<object, IDisposable>(); 

        private IConsumerWrapper consumer;

        public DynamicStoreAccessor(IStoreHolder<TStore> storeHolder)
        {
            if (!typeof(TStore).IsInterface)
            {
                throw new Exception("State type must be an interface");
            }
            this.storeHolder = storeHolder;

            DynamicStateProperty dynamicState = DynamicStateProperty.Create(storeHolder.RootObservableProperty);
            Store = DynamicStateProperty.Box<TStore>(dynamicState);

            PropertyAccessedEvent += DynamicStoreAccessor_PropertyAccessedEvent;
            storeHolder.StatePropertyChangedEvent += StoreHolder_StatePropertyChangedEvent;
            storeHolder.CollectionItemsChangedEvent += StoreHolder_CollectionItemsChangedEvent;

            PlantListener(dynamicState);
        }

        public TStore Store { get; }

        public void SetConsumer(BlazorMobxComponentBase<TStore> consumer)
        {
            this.consumer = new MobxConsumerWrapper(consumer);
        }

        public void SetConsumer(ComponentBase consumer)
        {
            this.consumer = new ReflectionConsumerWrapper(consumer);
        }

        public T CreateObservable<T>()
             where T : class
        {
            ObservableProperty observableProperty = storeHolder.CreateObservableProperty(typeof(T));
            DynamicStateProperty dynamicState = DynamicStateProperty.Create(observableProperty);

            return DynamicStateProperty.Box<T>(dynamicState);
        }

        public void Dispose()
        {
            PropertyAccessedEvent -= DynamicStoreAccessor_PropertyAccessedEvent;
            storeHolder.StatePropertyChangedEvent -= StoreHolder_StatePropertyChangedEvent;
            storeHolder.CollectionItemsChangedEvent -= StoreHolder_CollectionItemsChangedEvent;

            rootObserverDisposer?.Dispose();

            foreach (var observerDisposable in collectionItemObserverDisposables.Values)
            {
                observerDisposable.Dispose();
            }

            subscribedProperties = null;
            collectionItemObserverDisposables = null;
            consumer = null;
        }

        private void PlantListener(DynamicStateProperty dynamicState)
        {
            Contract.Assert(rootObserverDisposer == null);
            rootObserverDisposer = dynamicState.Subscribe(new PropertyChangedObserver(
                PropertyAccessedEvent,
                () => rootObserverDisposer = null));
        }

        private void DynamicStoreAccessor_PropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            subscribedProperties.Add((e.ObservedType, e.PropertyName));
        }

        private async void StoreHolder_StatePropertyChangedEvent(object sender, StatePropertyChangedEventArgs e)
        {
            ObservableProperty observableProperty = (ObservableProperty)sender;

            if (subscribedProperties.Contains((observableProperty.ObservedType, e.PropertyName)))
            {
                await consumer.ForceUpdate();
            }
        }

        private async void StoreHolder_CollectionItemsChangedEvent(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (object addedItem in e.ItemsAdded)
            {
                if (DynamicStateProperty.Unbox(addedItem) is DynamicStateProperty dynamicState)
                {
                    IDisposable disposable = dynamicState.Subscribe(new PropertyChangedObserver(
                        PropertyAccessedEvent,
                        () => collectionItemObserverDisposables.Remove(addedItem)));
                    collectionItemObserverDisposables.Add(addedItem, disposable);
                }
            }

            foreach (object removedItem in e.ItemsRemoved)
            {
                if (collectionItemObserverDisposables.ContainsKey(removedItem))
                {
                    collectionItemObserverDisposables[removedItem].Dispose();
                }
            }

            if (e.NewCount != e.OldCount || e.ItemsAdded.Any() || e.ItemsRemoved.Any())
            {
                await consumer.ForceUpdate();
            }
        }

        private class PropertyChangedObserver : IObserver<PropertyAccessedArgs>
        {
            private readonly EventHandler<PropertyAccessedEventArgs> propertyAccessedEvent;
            private readonly Action disposeAction;

            public PropertyChangedObserver(
                EventHandler<PropertyAccessedEventArgs> propertyAccessedEvent,
                Action disposeAction)
            {
                this.propertyAccessedEvent = propertyAccessedEvent;
                this.disposeAction = disposeAction;
            }

            public void OnNext(PropertyAccessedArgs value)
            {
                propertyAccessedEvent?.Invoke(this, new PropertyAccessedEventArgs
                {
                    ObservedType = value.ObservableProperty.ObservedType,
                    PropertyName = value.PropertyName
                });
            }

            public void OnCompleted()
            {
                disposeAction();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }
        }

        private interface IConsumerWrapper
        {
            Task ForceUpdate();
        }

        private class MobxConsumerWrapper : IConsumerWrapper
        {
            private readonly BlazorMobxComponentBase<TStore> consumer;

            public MobxConsumerWrapper(BlazorMobxComponentBase<TStore> consumer)
            {
                this.consumer = consumer;
            }

            public Task ForceUpdate()
            {
                return consumer.ForceUpdate();
            }
        }

        private class ReflectionConsumerWrapper : IConsumerWrapper
        {
            private static Func<ComponentBase, Action, Task> ComponentBaseInvokeAsync;
            private static Action<ComponentBase> ComponentBaseStateHasChanged;
            
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


            private readonly ComponentBase consumer;

            public ReflectionConsumerWrapper(ComponentBase consumer)
            {
                this.consumer = consumer;
            }

            public Task ForceUpdate()
            {
                return ComponentBaseInvokeAsync(consumer, () => ComponentBaseStateHasChanged(consumer));
            }
        }
    }
}
