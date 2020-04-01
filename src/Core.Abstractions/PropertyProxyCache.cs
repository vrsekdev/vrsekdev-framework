using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public class PropertyProxyCache<T> : IPropertyProxyCache<T>
        where T : class
    {
        public static IPropertyProxyCache<T> Create(
            IPropertyProxyWrapper proxyWrapper,
            IPropertyProxyFactory proxyFactory,
            IObservableFactory observableFactory)
        {
            return new PropertyProxyCache<T>(proxyWrapper, proxyFactory, observableFactory);
        }

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
            subscribers.Add(subscriber);
            foreach (var boxedValue in boxedValuesDictionary.Values)
            {
                IPropertyProxy proxy = proxyWrapper.UnwrapPropertyObservable((object)boxedValue);
                proxy.Subscribe(subscriber);
            }
        }

        private T BoxItem(T item)
        {
            if (!(item != null && proxyWrapper.UnwrapPropertyObservable(item) is IPropertyProxy propertyProxy))
            {
                IObservableProperty observableProperty = observableFactory.CreateObservableProperty(typeof(T));
                if (item != null)
                {
                    observableProperty.OverwriteFrom(item, true);
                }
                propertyProxy = proxyFactory.Create(observableProperty);
            }

            foreach(var subscriber in subscribers)
            {
                propertyProxy.Subscribe(subscriber);
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
