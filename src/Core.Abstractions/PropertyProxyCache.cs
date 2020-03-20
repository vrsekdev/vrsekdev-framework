using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public class PropertyProxyCache<T> : IPropertyProxyCache<T>
        where T : class
    {
        private readonly IPropertyProxyWrapper proxyWrapper;
        private readonly IPropertyProxyFactory proxyFactory;
        private readonly IObservableFactory observableFactory;

        // TODO: Concurrent dictionary
        private readonly List<IPropertyAccessedSubscriber> subscribers = new List<IPropertyAccessedSubscriber>();
        private readonly Dictionary<T, T> boxedValuesDictionary = new Dictionary<T, T>();


        public PropertyProxyCache(
            IPropertyProxyWrapper proxyWrapper,
            IPropertyProxyFactory proxyFactory,
            IObservableFactory observableFactory)
        {
            this.proxyWrapper = proxyWrapper;
            this.proxyFactory = proxyFactory;
            this.observableFactory = observableFactory;
        }

        public bool TryGetValue(T originalValue, out T value)
        {
            return boxedValuesDictionary.TryGetValue(originalValue, out value);
        }

        public T Insert(T originalValue)
        {
            T boxedValue = BoxItem(originalValue);
            boxedValuesDictionary.Add(originalValue, boxedValue);

            return boxedValue;
        }

        public bool Remove(T boxedValue)
        {
            return boxedValuesDictionary.Remove(boxedValue);
        }

        void IPropertyProxyCache<T>.SubscribeAll(IPropertyAccessedSubscriber subscriber)
        {
            foreach (var boxedValue in boxedValuesDictionary.Values)
            {
                IPropertyProxy proxy = proxyWrapper.UnwrapPropertyObservable((object)boxedValue);
                proxy.Subscribe(subscriber);
            }
        }

        private T BoxItem(T item)
        {
            IObservableProperty observableProperty;
            if (item != null && proxyWrapper.UnwrapPropertyObservable((object)item) is IPropertyProxy propertyProxy)
            {
                observableProperty = propertyProxy.ObservableProperty;
            }
            else
            {
                observableProperty = observableFactory.CreateObservableProperty(typeof(T));
                if (item != null)
                {
                    observableProperty.OverwriteFrom(item);
                }
            }

            propertyProxy = proxyFactory.Create(observableProperty);
            foreach (var observer in subscribers)
            {
                propertyProxy.Subscribe(observer);
            }

            return proxyWrapper.WrapPropertyObservable<T>(propertyProxy);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return boxedValuesDictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return boxedValuesDictionary.Values.GetEnumerator();
        }
    }
}
