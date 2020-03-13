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
        private readonly Dictionary<string, DynamicCollectionObservable> collectionObservables = new Dictionary<string, DynamicCollectionObservable>();
        private readonly Dictionary<string, object> propertyObservables = new Dictionary<string, object>();

        private readonly IObservableFactory observableFactory;

        public IObservableProperty ObservableProperty { get; }

        private DynamicPropertyObservable(
            IObservableProperty observableProperty)
        {
            ObservableProperty = observableProperty;
            observableFactory = observableProperty.CreateFactory();

            Initialize();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;

            OnPropertyAccessed(name);
            
            if (propertyObservables.TryGetValue(name, out object propertyObservable))
            {
                result = propertyObservable;

                return true;
            }

            if (collectionObservables.TryGetValue(name, out DynamicCollectionObservable collectionObservable))
            {
                result = collectionObservable;

                return true;
            }
            
            return ObservableProperty.TryGetMember(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name;

            if (propertyObservables.ContainsKey(name))
            {
                if (Unbox(value) is DynamicPropertyObservable newObservable)
                {
                    newObservable.Dispose();
                    return ObservableProperty.TrySetMember(name, newObservable.ObservableProperty);
                }
            }

            if (collectionObservables.TryGetValue(name, out DynamicCollectionObservable dynamicCollectionObservable))
            {
                bool result;
                if (result = ObservableProperty.TrySetMember(name, value))
                {
                    dynamicCollectionObservable.Reset();
                }

                return result;
            }

            return ObservableProperty.TrySetMember(name, value);
        }

        public IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer)
        {
            var disposer = new ObserverDisposer();

            foreach (var observedDynamicObject in propertyObservables.Values)
            {
                DynamicPropertyObservable dynamicState = Unbox(observedDynamicObject);
                if (dynamicState != null)
                {
                    disposer.AddDisposeAction(dynamicState.Subscribe(observer));
                }
            }

            foreach (var observedDynamicArray in collectionObservables.Values)
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
            foreach (var observedDynamicProperty in propertyObservables.Values)
            {
                DynamicPropertyObservable dynamicState = Unbox(observedDynamicProperty);
                dynamicState.Dispose();
            }

            foreach (var observedDynamicArray in collectionObservables.Values)
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
                propertyObservables[observedProperty.Key] = CreatePropertyObservable(observedProperty.Value);
            }

            foreach (var observedArray in observedArrays)
            {
                collectionObservables[observedArray.Key] = CreateCollectionObservable(observedArray.Value);
            }
        }

        private object CreatePropertyObservable(IObservableProperty observableProperty)
        {
            DynamicPropertyObservable dynamicState = new DynamicPropertyObservable(observableProperty);
            object boxedItem = Box(dynamicState, observableProperty.ObservedType).Target;

            foreach (var observer in observers)
            {
                observer.Value.AddDisposeAction(dynamicState.Subscribe(observer.Key));
            }

            return boxedItem;
        }

        private DynamicCollectionObservable CreateCollectionObservable(IObservableCollection observableArray)
        {
            Type elementType = observableArray.ElementType;
            Type dynamicObserverArrayType = typeof(DynamicCollectionObservable<>).MakeGenericType(elementType);

            DynamicCollectionObservable dynamicCollectionObservable = (DynamicCollectionObservable)Activator.CreateInstance(
                dynamicObserverArrayType, 
                observableArray, 
                observableFactory);

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
