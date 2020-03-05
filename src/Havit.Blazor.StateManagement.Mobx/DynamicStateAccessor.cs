using Havit.Blazor.StateManagement.Mobx.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    public class PropertyAccessedEventArgs
    {
        public Type ObservedType { get; set; }

        public string PropertyName { get; set; }
    }

    public class DynamicStateAccessor<TState> : IStateAccessor<TState>, IDisposable
        where TState : class
    {
        private readonly HashSet<(Type, string)> subscribedProperties = new HashSet<(Type, string)>();

        private IDisposable observerDisposer;
        private BlazorMobxComponentBase<TState> consumer;
        private event EventHandler<PropertyAccessedEventArgs> PropertyAccessedEvent;

        public DynamicStateAccessor(IStateHolder<TState> stateHolder)
        {
            if (!typeof(TState).IsInterface)
            {
                throw new Exception("State type must be an interface");
            }

            DynamicStateProperty dynamicState = DynamicStateProperty.Create(stateHolder.RootObservableProperty);
            State = DynamicStateProperty.Box<TState>(dynamicState);

            PropertyAccessedEvent += DynamicStateAccessor_PropertyAccessedEvent;
            stateHolder.StatePropertyChangedEvent += StateHolder_StatePropertyChangedEvent;
            stateHolder.CollectionItemsChangedEvent += StateHolder_CollectionItemsChangedEvent;

            PlantListener(dynamicState);
        }

        public TState State { get; }

        public void SetConsumer(BlazorMobxComponentBase<TState> consumer)
        {
            this.consumer = consumer;
        }

        public void Dispose()
        {
            if (observerDisposer != null)
            {
                observerDisposer.Dispose();
            }
        }

        private void PlantListener(DynamicStateProperty dynamicState)
        {
            Contract.Assert(observerDisposer == null);
            observerDisposer = dynamicState.Subscribe(new PropertyChangedObserver(PropertyAccessedEvent));
        }

        private void DynamicStateAccessor_PropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            subscribedProperties.Add((e.ObservedType, e.PropertyName));
        }

        private async void StateHolder_StatePropertyChangedEvent(object sender, StatePropertyChangedEventArgs e)
        {
            ObservableProperty observableProperty = (ObservableProperty)sender;

            if (subscribedProperties.Contains((observableProperty.ObservedType, e.PropertyName)))
            {
                await consumer.ForceUpdate();
            }
        }

        private async void StateHolder_CollectionItemsChangedEvent(object sender, CollectionItemsChangedEventArgs e)
        {
            if (e.NewCount != e.OldCount)
            {
                await consumer.ForceUpdate();
            }
        }

        private class PropertyChangedObserver : IObserver<PropertyAccessedArgs>
        {
            private readonly EventHandler<PropertyAccessedEventArgs> propertyAccessedEvent;

            public PropertyChangedObserver(
                EventHandler<PropertyAccessedEventArgs> propertyAccessedEvent)
            {
                this.propertyAccessedEvent = propertyAccessedEvent;
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
                throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }
        }
    }
}
