using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy
{
    public class RuntimeProxyManager<TInterface> : IRuntimeProxyManager
        where TInterface : class
    {
        #region static
        private readonly static Lazy<Type> runtimeTypeLazy;
        private readonly static Func<Type, IObservableProperty, IRuntimeProxyManager> createProxy;

        static RuntimeProxyManager()
        {
            Type type = typeof(RuntimeProxyManager<TInterface>);

            /**** RuntimeType ****/
            MethodInfo getMethod = type.GetMethod(nameof(GetValue));
            MethodInfo setMethod = type.GetMethod(nameof(SetValue));
            runtimeTypeLazy = new Lazy<Type>(() => RuntimeProxyBuilder.BuildRuntimeType(typeof(TInterface), getMethod, setMethod));

            /**** CreateManagerInternal ****/
            MethodInfo createInternalInfo = type.GetMethod(nameof(CreateManagerInternal), BindingFlags.Static | BindingFlags.NonPublic);
            createProxy = (type, prop) => (IRuntimeProxyManager)createInternalInfo.MakeGenericMethod(type).Invoke(null, new[] { prop });
        }

        private static IRuntimeProxyManager CreateManagerInternal<T>(IObservableProperty observableProperty)
            where T : class
        {
            return new RuntimeProxyManager<T>(observableProperty);
        }
        #endregion static

        private readonly IObservableFactory observableFactory;
        private readonly RuntimeProxyWrapper proxyWrapper = new RuntimeProxyWrapper();
        private readonly RuntimeProxyFactory proxyFactory = new RuntimeProxyFactory();

        private readonly HashSet<IPropertyAccessedSubscriber> subscribers = new HashSet<IPropertyAccessedSubscriber>();
        private readonly Dictionary<string, ICollectionProxy> collectionProxies = new Dictionary<string, ICollectionProxy>();
        private readonly Dictionary<string, IRuntimeProxyManager> runtimeProxies = new Dictionary<string, IRuntimeProxyManager>();

        public IObservableProperty ObservableProperty { get; }

        private WeakReference<TInterface> implementation;
        internal TInterface Implementation
        {
            get
            {
                TInterface impl;

                if (implementation == null)
                {
                    impl = (TInterface)Activator.CreateInstance(runtimeTypeLazy.Value, new object[] { this });
                    implementation = new WeakReference<TInterface>(impl);
                    return impl;
                }

                if (!implementation.TryGetTarget(out impl))
                {
                    throw new ObjectDisposedException(nameof(impl));
                }

                return impl;
            }
        }

        object IRuntimeProxyManager.Implementation => Implementation;

        public RuntimeProxyManager(
            IObservableProperty observableProperty)
        {
            ObservableProperty = observableProperty;
            observableFactory = observableProperty.CreateFactory();

            Initialize();
        }

        public object GetValue(string propertyName)
        {
            OnPropertyAccessed(propertyName);

            if (runtimeProxies.TryGetValue(propertyName, out IRuntimeProxyManager propertyManager))
            {
                return propertyManager.Implementation;
            }

            if (collectionProxies.TryGetValue(propertyName, out ICollectionProxy collectionProxy))
            {
                return collectionProxy;
            }

            if (!ObservableProperty.TryGetMember(propertyName, out object result))
            {
                throw new NotSupportedException(propertyName);
            }

            return result;
        }

        public void SetValue(string propertyName, object value)
        {
            if (runtimeProxies.ContainsKey(propertyName))
            {
                if (value is IRuntimeProxy runtimeProxy)
                {
                    if (!ObservableProperty.TrySetMember(propertyName, runtimeProxy.Manager.ObservableProperty))
                    {
                        throw new NotSupportedException(propertyName);
                    }
                }
            }

            if (collectionProxies.TryGetValue(propertyName, out ICollectionProxy collectionProxy))
            {
                if (ObservableProperty.TrySetMember(propertyName, value))
                {
                    collectionProxy.Reset();
                }
#if DEBUG
                else
                {
                    throw new Exception();
                }
#endif
            }

            if (!ObservableProperty.TrySetMember(propertyName, value))
            {
                throw new NotSupportedException(propertyName);
            }
        }

        public void Subscribe(IPropertyAccessedSubscriber subscriber)
        {
            subscribers.Add(subscriber);

            foreach (var runtimeProxy in runtimeProxies.Values)
            {
                runtimeProxy.Subscribe(subscriber);
            }

            foreach (var collectionProxy in collectionProxies.Values)
            {
                collectionProxy.Subscribe(subscriber);
            }
        }

        private void Initialize()
        {
            var observedProperties = ObservableProperty.GetObservedProperties();
            var observedCollections = ObservableProperty.GetObservedCollections();

            foreach (var observedProperty in observedProperties)
            {
                IObservableProperty observableProperty = observedProperty.Value;
                runtimeProxies[observedProperty.Key] = createProxy(observableProperty.ObservedType, observableProperty);
            }

            foreach (var observedCollection in observedCollections)
            {
                collectionProxies[observedCollection.Key] = CollectionProxy.Create(
                    observedCollection.Value,
                    proxyWrapper,
                    proxyFactory,
                    observableFactory);
            }
        }

        private void OnPropertyAccessed(string name)
        {
            var args = new PropertyAccessedArgs
            {
                PropertyProxy = this,
                PropertyName = name
            };

            foreach (var subscriber in subscribers.ToList())
            {
                subscriber.OnPropertyAccessed(args);
            }
        }
    }
}
