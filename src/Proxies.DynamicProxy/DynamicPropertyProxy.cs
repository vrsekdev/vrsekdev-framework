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

        private readonly HashSet<IPropertyAccessedSubscriber> subscribers = new HashSet<IPropertyAccessedSubscriber>();
        private readonly Dictionary<string, ICollectionProxy> collectionProxies = new Dictionary<string, ICollectionProxy>();
        private readonly Dictionary<string, object> dynamicProxies = new Dictionary<string, object>();

        private readonly IObservableFactory observableFactory;
        private readonly DynamicProxyWrapper proxyWrapper = new DynamicProxyWrapper();
        private readonly DynamicProxyFactory proxyFactory = new DynamicProxyFactory();

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

            if (collectionProxies.TryGetValue(name, out ICollectionProxy collectionObservable))
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
                    return ObservableProperty.TrySetMember(name, newProxy.ObservableProperty);
                }
            }

            if (collectionProxies.TryGetValue(name, out ICollectionProxy dynamicCollectionObservable))
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

        public void Subscribe(IPropertyAccessedSubscriber subscriber)
        {
            foreach (var observedDynamicObject in dynamicProxies.Values)
            {
                DynamicPropertyProxy dynamicProxy = Unbox(observedDynamicObject);
                if (dynamicProxy != null)
                {
                    dynamicProxy.Subscribe(subscriber);
                }
            }

            foreach (var observedDynamicArray in collectionProxies.Values)
            {
                observedDynamicArray.Subscribe(subscriber);
            }
        }

        public override string ToString()
        {
            return ObservableProperty.ToString();
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
                collectionProxies[observedArray.Key] = CreateCollectionObservable(observedArray.Value);
            }
        }

        private object CreatePropertyObservable(IObservableProperty observableProperty)
        {
            DynamicPropertyProxy dynamicProxy = new DynamicPropertyProxy(observableProperty);
            object boxedItem = Box(dynamicProxy, observableProperty.ObservedType).Target;

            foreach (var subscriber in subscribers)
            {
                dynamicProxy.Subscribe(subscriber);
            }

            return boxedItem;
        }

        private ICollectionProxy CreateCollectionObservable(IObservableCollection observableCollection)
        {
            ICollectionProxy collectionProxy = CollectionProxy.Create(
                observableCollection, 
                proxyWrapper,
                proxyFactory, 
                observableFactory);

            foreach (var subscriber in subscribers)
            {
                collectionProxy.Subscribe(subscriber);
            }

            return collectionProxy;
        }

        private void OnPropertyAccessed(string name)
        {
            var args = new PropertyAccessedArgs
            {
                PropertyProxy = this,
                PropertyName = name
            };

            foreach (var observer in subscribers.ToList())
            {
                observer.OnPropertyAccessed(args);
            }
        }
    }
}
