using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy
{
    #region helper
    internal delegate IRuntimeProxyManager CreateRuntimeManager(IObservableProperty observableProperty, bool readOnly);
    internal static class RuntimeProxyManagerHelper
    {
        public readonly static CreateRuntimeManager CreateRuntimeManager;

        static RuntimeProxyManagerHelper()
        {
            Type type = typeof(RuntimeProxyManager<>);
            CreateRuntimeManager =
                (IObservableProperty observableProperty, bool readOnly) =>
                {
                    Type genericType = type.MakeGenericType(observableProperty.ObservedType);
                    return (IRuntimeProxyManager)Activator.CreateInstance(genericType, new object[] { observableProperty, readOnly });
                };
        }
    }
    #endregion helper

    public class RuntimeProxyManager<TInterface> : IRuntimeProxyManager
        where TInterface : class
    {
        #region static
        private readonly static Lazy<Type> runtimeTypeLazy;

        static RuntimeProxyManager()
        {
            Type currentType = typeof(RuntimeProxyManager<TInterface>);

            /**** RuntimeType ****/
            MethodInfo getMethod = currentType.GetMethod(nameof(GetValue));
            MethodInfo setMethod = currentType.GetMethod(nameof(SetValue));
            runtimeTypeLazy = new Lazy<Type>(() => RuntimeProxyBuilder.BuildRuntimeType(typeof(TInterface), getMethod, setMethod));
        }
        #endregion static

        private readonly IObservableFactory observableFactory;
        private readonly RuntimeProxyWrapper proxyWrapper = new RuntimeProxyWrapper();
        private readonly RuntimeProxyFactory proxyFactory = new RuntimeProxyFactory();

        private readonly HashSet<IPropertyAccessedSubscriber> subscribers = new HashSet<IPropertyAccessedSubscriber>();
        private readonly Dictionary<string, ICollectionProxy> collectionProxies = new Dictionary<string, ICollectionProxy>();
        /// <summary>
        /// Manager has to keep track of all nested proxies, otherwise when the (for example) component keeps track of only the root proxy, 
        /// all nested proxies lose reference and get collected by GC, because manager keeps only a weak reference
        /// </summary>
        private readonly Dictionary<string, IRuntimeProxy> runtimeProxies = new Dictionary<string, IRuntimeProxy>();

        public IObservableProperty ObservableProperty { get; }
        public bool IsReadOnly { get; }

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
            IObservableProperty observableProperty,
            bool readOnly)
        {
            ObservableProperty = observableProperty;
            IsReadOnly = readOnly;
            observableFactory = observableProperty.CreateFactory();

            Initialize();
        }

        public object GetValue(string propertyName)
        {
            OnPropertyAccessed(propertyName);

            if (runtimeProxies.TryGetValue(propertyName, out IRuntimeProxy runtimeProxy))
            {
                return runtimeProxy.Manager.Implementation;
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
            if (IsReadOnly)
            {
                throw new PropertyReadonlyException();
            }

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
                runtimeProxy.Manager.Subscribe(subscriber);
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
                runtimeProxies[observedProperty.Key] = (IRuntimeProxy)RuntimeProxyManagerHelper.CreateRuntimeManager(observableProperty, IsReadOnly).Implementation;
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
