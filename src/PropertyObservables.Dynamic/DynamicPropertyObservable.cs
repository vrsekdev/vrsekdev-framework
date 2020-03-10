using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Dynamic;
using System.Linq;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.Dynamic
{
    internal class DynamicPropertyObservable : DynamicObject, IPropertyObservable
    {
        public static DynamicPropertyObservable Create(IObservableProperty observableProperty)
        {
            return new DynamicPropertyObservable(observableProperty);
        }

        public static T Box<T>(DynamicPropertyObservable dynamicState)
            where T : class
        {
            return ImpromptuInterface.Impromptu.ActLike<T>(dynamicState);
        }

        public static dynamic Box(DynamicPropertyObservable dynamicState, Type type)
        {
            return ImpromptuInterface.Impromptu.ActLike(dynamicState, type);
        }

        public static DynamicPropertyObservable Unbox<T>(T val)
        {
            return ImpromptuInterface.Impromptu.UndoActLike(val) as DynamicPropertyObservable;
        }

        private readonly Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer> observers = new Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer>();

        private readonly Dictionary<string, DynamicCollectionObservable> observedDynamicArrays = new Dictionary<string, DynamicCollectionObservable>();
        private readonly Dictionary<string, object> observedDynamicProperties = new Dictionary<string, object>();

        public IObservableProperty ObservableProperty { get; }
        private IObservableFactory ObservableFactory { get; }

        private DynamicPropertyObservable(
            IObservableProperty observableProperty)
        {
            ObservableProperty = observableProperty;
            ObservableFactory = observableProperty.CreateFactory();

            Initialize();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;

            OnPropertyAccessed(name);
            
            if (observedDynamicProperties.ContainsKey(name))
            {
                result = observedDynamicProperties[name];

                return true;
            }

            if (observedDynamicArrays.ContainsKey(name))
            {
                result = observedDynamicArrays[name];

                return true;
            }
            
            return ObservableProperty.TryGetMember(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name;

            if (observedDynamicArrays.TryGetValue(name, out DynamicCollectionObservable dynamicObservableArray))
            {
                bool result;
                if (result = ObservableProperty.TrySetMember(name, value))
                {
                    dynamicObservableArray.Reset();
                }

                return result;
            }

            return ObservableProperty.TrySetMember(name, value);
        }

        public IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer)
        {
            var disposer = new ObserverDisposer();

            foreach (var observedDynamicObject in observedDynamicProperties.Values)
            {
                DynamicPropertyObservable dynamicState = Unbox(observedDynamicObject);
                if (dynamicState != null)
                {
                    disposer.AddDisposeAction(dynamicState.Subscribe(observer));
                }
            }

            foreach (var observedDynamicArray in observedDynamicArrays.Values)
            {
                disposer.AddDisposeAction(observedDynamicArray.Subscribe(observer));
            }

            disposer.AddDisposeAction(() => observers.Remove(observer));

            observers.Add(observer, disposer);
            return disposer;
        }

        public override string ToString()
        {
            return ObservableProperty.ToString();
        }

        public void Dispose()
        {
            foreach (var observedDynamicProperty in observedDynamicProperties.Values)
            {
                DynamicPropertyObservable dynamicState = Unbox(observedDynamicProperty);
                dynamicState.Dispose();
            }

            foreach (var observedDynamicArray in observedDynamicArrays.Values)
            {
                observedDynamicArray.Dispose();
            }

            foreach (var observer in observers.Keys)
            {
                observer.OnCompleted();
            }
        }

        private void Initialize()
        {
            var observedProperties = ObservableProperty.GetObservedProperties();
            var observedArrays = ObservableProperty.GetObservedCollections();

            foreach (var observedProperty in observedProperties)
            {
                observedDynamicProperties[observedProperty.Key] = CreateObserverProperty(observedProperty.Value);
            }

            foreach (var observedArray in observedArrays)
            {
                observedDynamicArrays[observedArray.Key] = CreateObserverArray(observedArray.Value);
            }
        }

        private object CreateObserverProperty(IObservableProperty observableProperty)
        {
            DynamicPropertyObservable dynamicState = new DynamicPropertyObservable(observableProperty);
            object boxedItem = Box(dynamicState, observableProperty.ObservedType).Target;

            foreach (var observer in observers)
            {
                observer.Value.AddDisposeAction(dynamicState.Subscribe(observer.Key));
            }

            return boxedItem;
        }

        private DynamicCollectionObservable CreateObserverArray(IObservableCollection observableArray)
        {
            Type elementType = observableArray.ElementType;
            Type dynamicObserverArrayType = typeof(DynamicCollectionObservable<>).MakeGenericType(elementType);

            DynamicCollectionObservable dynamicCollectionObservable = (DynamicCollectionObservable)Activator.CreateInstance(
                dynamicObserverArrayType, 
                observableArray, 
                ObservableFactory);

            foreach (var observer in observers)
            {
                observer.Value.AddDisposeAction(dynamicCollectionObservable.Subscribe(observer.Key));
            }

            return dynamicCollectionObservable;
        }

        private void OnPropertyAccessed(string name)
        {
            var args = new PropertyAccessedArgs
            {
                PropertyObservable = this,
                PropertyName = name
            };

            foreach (var observer in observers.Keys.ToList())
            {
                observer.OnNext(args);
            }
        }
    }
}
