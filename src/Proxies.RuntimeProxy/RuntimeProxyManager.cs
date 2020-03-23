using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Exceptions;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Emit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy
{
    internal interface IRuntimeProxyManager : IPropertyProxy
    {
        void SetDefaultValue(string propertyName, object value);

        object Implementation { get; }
    }

    #region helper
    internal delegate IRuntimeProxyManager CreateRuntimeManager(IObservableProperty observableProperty, MethodInterceptions methodInterceptions, bool readOnly);
    internal static class RuntimeProxyManagerHelper
    {
        public readonly static CreateRuntimeManager CreateRuntimeManager;

        static RuntimeProxyManagerHelper()
        {
            Type type = typeof(RuntimeProxyManager<>);
            CreateRuntimeManager =
                (observableProperty, methodInterceptions, readOnly) =>
                {
                    Type genericType = type.MakeGenericType(observableProperty.ObservedType);
                    return (IRuntimeProxyManager)Activator.CreateInstance(genericType, new object[] { observableProperty, methodInterceptions, readOnly });
                };
        }
    }
    #endregion helper

    internal class RuntimeProxyManager<TInterface> : IRuntimeProxyManager
        where TInterface : class
    {
        #region static
        private readonly static Lazy<Type> runtimeTypeWithoutInterceptions;

        private readonly static MethodInfo getMethod;
        private readonly static MethodInfo setMethod;

        static RuntimeProxyManager()
        {
            Type currentType = typeof(RuntimeProxyManager<TInterface>);

            /**** RuntimeType ****/
            getMethod = currentType.GetMethod(nameof(GetValue));
            setMethod = currentType.GetMethod(nameof(SetValue));
            runtimeTypeWithoutInterceptions = new Lazy<Type>(() => RuntimeProxyBuilder.BuildRuntimeType(typeof(TInterface), getMethod, setMethod, null));
        }

        private static Type GetRuntimeType(MethodInterceptions interceptions)
        {
            if (interceptions == null)
            {
                return runtimeTypeWithoutInterceptions.Value;
            }

            return RuntimeProxyBuilder.BuildRuntimeType(typeof(TInterface), getMethod, setMethod, interceptions);
        }
        #endregion static

        private readonly RuntimeProxyWrapper proxyWrapper = new RuntimeProxyWrapper();
        private readonly RuntimeProxyFactory proxyFactory = new RuntimeProxyFactory();

        private readonly HashSet<IPropertyAccessedSubscriber> subscribers = new HashSet<IPropertyAccessedSubscriber>();
        private readonly Dictionary<string, ICollectionProxy> collectionProxies = new Dictionary<string, ICollectionProxy>();
        /// <summary>
        /// Manager has to keep track of all nested proxies, otherwise when the (for example) component keeps track of only the root proxy, 
        /// all nested proxies lose reference and get collected by GC, because manager keeps only a weak reference
        /// </summary>
        private readonly Dictionary<string, IRuntimeProxy> runtimeProxies = new Dictionary<string, IRuntimeProxy>();

        private readonly IObservableFactory observableFactory;
        private readonly MethodInterceptions methodInterceptions;

        #region ctors
        public RuntimeProxyManager(
            IObservableProperty observableProperty) : this(observableProperty, null, false)
        {
        }

        public RuntimeProxyManager(
            IObservableProperty observableProperty,
            bool readOnly) : this(observableProperty, null, readOnly)
        {
        }

        public RuntimeProxyManager(
            IObservableProperty observableProperty,
            MethodInterceptions methodInterceptions) : this(observableProperty, methodInterceptions, false)
        {
        }

        public RuntimeProxyManager(
            IObservableProperty observableProperty,
            MethodInterceptions methodInterceptions,
            bool readOnly)
        {
            ObservableProperty = observableProperty;
            this.methodInterceptions = methodInterceptions;
            IsReadOnly = readOnly;
            observableFactory = observableProperty.CreateFactory();

            Initialize();
        }
        #endregion ctors

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
                    Type runtimeType = GetRuntimeType(methodInterceptions);
                    impl = (TInterface)Activator.CreateInstance(runtimeType, new object[] { this, methodInterceptions });
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

        public void SetDefaultValue(string propertyName, object value)
        {
            if (runtimeProxies.TryGetValue(propertyName, out IRuntimeProxy runtimeProxy))
            {
                runtimeProxy.Manager.ObservableProperty.OverwriteFrom(value);
            }

            if (collectionProxies.TryGetValue(propertyName, out ICollectionProxy collectionProxy))
            {
                collectionProxy.AddDefaultElements((IEnumerable)value);
            }

            if (!ObservableProperty.TrySetDefaultValue(propertyName, value))
            {
                throw new NotSupportedException(propertyName);
            }
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
                runtimeProxies[observedProperty.Key] = (IRuntimeProxy)RuntimeProxyManagerHelper.CreateRuntimeManager(observableProperty, null, IsReadOnly).Implementation;
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
