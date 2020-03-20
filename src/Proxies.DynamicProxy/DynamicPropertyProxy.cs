using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Dynamic;
using System.Linq;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.DynamicProxy
{
    internal class DynamicPropertyProxy : DynamicObject, IPropertyProxy
    {
        public static DynamicPropertyProxy Create(IObservableProperty observableProperty)
        {
            return new DynamicPropertyProxy(observableProperty);
        }

        public static T Box<T>(DynamicPropertyProxy dynamicProxy)
            where T : class
        {
            return ImpromptuInterface.Impromptu.ActLike<T>(dynamicProxy);
        }

        public static dynamic Box(DynamicPropertyProxy dynamicProxy, Type type)
        {
            return ImpromptuInterface.Impromptu.ActLike(dynamicProxy, type);
        }

        public static DynamicPropertyProxy Unbox<T>(T val)
        {
            return ImpromptuInterface.Impromptu.UndoActLike(val) as DynamicPropertyProxy;
        }

        private readonly Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer> observers = new Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer>();
        private readonly Dictionary<string, DynamicCollectionObservable> collectionObservables = new Dictionary<string, DynamicCollectionObservable>();
        private readonly Dictionary<string, object> dynamicProxies = new Dictionary<string, object>();

        private readonly IObservableFactory observableFactory;

        public IObservableProperty ObservableProperty { get; }

        private DynamicPropertyProxy(
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
            
            if (dynamicProxies.TryGetValue(name, out object dynamicProxy))
            {
                result = dynamicProxy;

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

            if (dynamicProxies.ContainsKey(name))
            {
                if (Unbox(value) is DynamicPropertyProxy newProxy)
                {
                    newProxy.Dispose();
                    return ObservableProperty.TrySetMember(name, newProxy.ObservableProperty);
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

            foreach (var observedDynamicObject in dynamicProxies.Values)
            {
                DynamicPropertyProxy dynamicProxy = Unbox(observedDynamicObject);
                if (dynamicProxy != null)
                {
                    disposer.AddDisposeAction(dynamicProxy.Subscribe(observer));
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
            foreach (var observedDynamicProperty in dynamicProxies.Values)
            {
                DynamicPropertyProxy dynamicProxy = Unbox(observedDynamicProperty);
                dynamicProxy.Dispose();
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
                dynamicProxies[observedProperty.Key] = CreatePropertyObservable(observedProperty.Value);
            }

            foreach (var observedArray in observedArrays)
            {
                collectionObservables[observedArray.Key] = CreateCollectionObservable(observedArray.Value);
            }
        }

        private object CreatePropertyObservable(IObservableProperty observableProperty)
        {
            DynamicPropertyProxy dynamicProxy = new DynamicPropertyProxy(observableProperty);
            object boxedItem = Box(dynamicProxy, observableProperty.ObservedType).Target;

            foreach (var observer in observers)
            {
                observer.Value.AddDisposeAction(dynamicProxy.Subscribe(observer.Key));
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
                PropertyProxy = this,
                PropertyName = name
            };

            foreach (var observer in observers.Keys.ToList())
            {
                observer.OnNext(args);
            }
        }
    }
}
