﻿using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType
{
    public class RuntimeTypePropertyObservableManager<TInterface> : IRuntimeTypePropertyObservableManager
        where TInterface : class
    {
        #region static
        private readonly static Lazy<Type> runtimeTypeLazy;
        private readonly static Func<Type, IObservableProperty, IRuntimeTypePropertyObservableManager> createManager;

        static RuntimeTypePropertyObservableManager()
        {
            Type type = typeof(RuntimeTypePropertyObservableManager<TInterface>);

            /**** RuntimeType ****/
            MethodInfo getMethod = type.GetMethod(nameof(GetValue));
            MethodInfo setMethod = type.GetMethod(nameof(SetValue));
            runtimeTypeLazy = new Lazy<Type>(() => RuntimeTypeBuilder.BuildRuntimeType(typeof(TInterface), getMethod, setMethod));

            /**** CreateManagerInternal ****/
            MethodInfo createInternalInfo = type.GetMethod(nameof(CreateManagerInternal), BindingFlags.Static | BindingFlags.NonPublic);
            createManager = (type, prop) => (IRuntimeTypePropertyObservableManager)createInternalInfo.MakeGenericMethod(type).Invoke(null, new[] { prop });
        }

        private static IRuntimeTypePropertyObservableManager CreateManagerInternal<T>(IObservableProperty observableProperty)
            where T : class
        {
            return new RuntimeTypePropertyObservableManager<T>(observableProperty);
        }
        #endregion static

        private readonly Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer> observers = new Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer>();
        private readonly Dictionary<string, RuntimeTypeCollectionObservable<TInterface>> collectionObservables = new Dictionary<string, RuntimeTypeCollectionObservable<TInterface>>();
        private readonly Dictionary<string, IRuntimeTypePropertyObservableManager> propertyObservables = new Dictionary<string, IRuntimeTypePropertyObservableManager>();

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

        object IRuntimeTypePropertyObservableManager.Implementation => Implementation;

        public RuntimeTypePropertyObservableManager(
            IObservableProperty observableProperty)
        {
            ObservableProperty = observableProperty;

            Initialize();
        }

        public object GetValue(string propertyName)
        {
            OnPropertyAccessed(propertyName);

            if (propertyObservables.TryGetValue(propertyName, out IRuntimeTypePropertyObservableManager propertyManager))
            {
                return propertyManager.Implementation;
            }

            if (collectionObservables.TryGetValue(propertyName, out RuntimeTypeCollectionObservable<TInterface> collectionObservable))
            {
                return collectionObservable;
            }

            if (!ObservableProperty.TryGetMember(propertyName, out object result))
            {
                throw new NotSupportedException(propertyName);
            }

            return result;
        }

        public void SetValue(string propertyName, object value)
        {
            if (propertyObservables.ContainsKey(propertyName))
            {
                if (value is IRuntimeTypeImpl newObservable)
                {
                    if (!ObservableProperty.TrySetMember(propertyName, newObservable.Manager.ObservableProperty))
                    {
                        throw new NotSupportedException(propertyName);
                    }
                }
            }

            if (collectionObservables.TryGetValue(propertyName, out RuntimeTypeCollectionObservable<TInterface> collectionObservable))
            {
                /*bool result;
                if (result = ObservableProperty.TrySetMember(name, value))
                {
                    collectionObservable.Reset();
                }

                return result;*/
            }

            if (!ObservableProperty.TrySetMember(propertyName, value))
            {
                throw new NotSupportedException(propertyName);
            }
        }

        public IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer)
        {
            ObserverDisposer disposer = new ObserverDisposer();

            foreach (var propertyManager in propertyObservables.Values)
            {
                disposer.AddDisposeAction(propertyManager.Subscribe(observer));
            }

            /*foreach (var observedDynamicArray in collectionObservables.Values)
            {
                disposer.AddDisposeAction(observedDynamicArray.Subscribe(observer));
            }*/

            disposer.AddDisposeAction(() => observers.Remove(observer));

            observers.Add(observer, disposer);

            return disposer;
        }

        private void Initialize()
        {
            var observedProperties = ObservableProperty.GetObservedProperties();
            var observedArrays = ObservableProperty.GetObservedCollections();

            foreach (var observedProperty in observedProperties)
            {
                IObservableProperty observableProperty = observedProperty.Value;
                propertyObservables[observedProperty.Key] = createManager(observableProperty.ObservedType, observableProperty);
            }

            foreach (var observedArray in observedArrays)
            {
                // TODO
                //collectionObservables[observedArray.Key] = CreateCollectionObservable(observedArray.Value);
            }
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
