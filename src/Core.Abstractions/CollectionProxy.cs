using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public class CollectionProxy
    {
        private delegate ICollectionProxy CollectionProxyFactory(
            IObservableCollection observableCollection,
            IPropertyProxyWrapper proxyWrapper,
            IPropertyProxyFactory proxyFactory,
            IObservableFactory observableFactory);

        private static CollectionProxyFactory collectionProxyFactory;

        static CollectionProxy()
        {
            collectionProxyFactory = (observableCollection, proxyWrapper, proxyFactory, observableFactory) =>
            {
                Type elementType = observableCollection.ElementType;
                Type collectionType = typeof(CollectionProxy<>).MakeGenericType(elementType);

                return (ICollectionProxy)Activator.CreateInstance(collectionType, new object[] { observableCollection, proxyWrapper, proxyFactory, observableFactory });
            };
        }

        public static ICollectionProxy Create(
            IObservableCollection observableCollection,
            IPropertyProxyWrapper proxyWrapper,
            IPropertyProxyFactory proxyFactory,
            IObservableFactory observableFactory)
        {
            return collectionProxyFactory(observableCollection, proxyWrapper, proxyFactory, observableFactory);
        }
    }

    public sealed class CollectionProxy<T> : CollectionProxy, IObservableCollection<T>, ICollectionProxy
    {
        #region static
        private delegate IPropertyProxyCache<T> CacheFactory(
            IPropertyProxyWrapper proxyWrapper,
            IPropertyProxyFactory proxyFactory,
            IObservableFactory observableFactory);

        private delegate Enumerator<T> ObservableEnumeratorFactory(
            IObservableCollection<T> observableCollection,
            IPropertyProxyCache<T> propertyProxyCache);

        private readonly static CacheFactory cacheFactory;
        private readonly static ObservableEnumeratorFactory observableEnumeratorFactory;

        static CollectionProxy()
        {
            cacheFactory = (proxyWrapper, proxyFactory, observableFactory) =>
            {
                Type cacheType = typeof(PropertyProxyCache<>).MakeGenericType(typeof(T));
                return (IPropertyProxyCache<T>)Activator
                    .CreateInstance(cacheType, new object[] { proxyWrapper, proxyFactory, observableFactory });
            };

            observableEnumeratorFactory = (observableCollection, propertyProxyCache) =>
            {
                Type type = typeof(ObservableEnumerator<>).MakeGenericType(typeof(T));
                return (Enumerator<T>)Activator
                    .CreateInstance(type, new object[] { observableCollection, propertyProxyCache });
            };
        }

        #endregion static

        private readonly IObservableCollection<T> observableCollection;

        private readonly IPropertyProxyWrapper proxyWrapper;
        private readonly IPropertyProxyFactory proxyFactory;
        private readonly IObservableFactory observableFactory;

        private IPropertyProxyCache<T> propertyProxyCache;

        public CollectionProxy(
            IObservableCollection<T> observableCollection,
            IPropertyProxyWrapper proxyWrapper,
            IPropertyProxyFactory proxyFactory,
            IObservableFactory observableFactory)
        {
            this.observableCollection = observableCollection;
            this.proxyWrapper = proxyWrapper;
            this.proxyFactory = proxyFactory;
            this.observableFactory = observableFactory;

            ElementType = typeof(T);
            ElementObserved = observableCollection.ElementObserved;
            if (ElementObserved)
            {
                propertyProxyCache = cacheFactory(proxyWrapper, proxyFactory, observableFactory);
            }
        }

        public T this[int index]
        {
            get
            {
                T originalValue = observableCollection[index];
                if (!propertyProxyCache.TryGetValue(originalValue, out T boxedValue))
                {
                    return propertyProxyCache.Insert(originalValue);
                }

                return boxedValue;
            }
            set => Insert(index, value);
        }

        public int Count => observableCollection.Count;
        public bool IsReadOnly => ((IList)observableCollection).IsReadOnly;

        public bool ElementObserved { get; }
        public Type ElementType { get; }

        public void Add(T item)
        {
            if (ElementObserved)
            {
                propertyProxyCache.Insert(item);
            }

            observableCollection.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (ElementObserved)
            {
                foreach (T item in items)
                {
                    propertyProxyCache.Insert(item);
                }
            }

            observableCollection.AddRange(items);
        }

        public void Clear()
        {
            Reset();

            observableCollection.Clear();
        }

        public void Reset()
        {
            if (ElementObserved)
            {
                propertyProxyCache = cacheFactory(proxyWrapper, proxyFactory, observableFactory);
            }
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var boxedItem in propertyProxyCache)
            {
                array[index] = boxedItem;
            }
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var boxedItem in propertyProxyCache)
            {
                array.SetValue(boxedItem, index);
            }
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            if (ElementObserved)
            {
                T previousOriginalValue = observableCollection[index];
                if (propertyProxyCache.TryGetValue(previousOriginalValue, out T previousValue))
                {
                    propertyProxyCache.Remove(previousValue);
                }

                propertyProxyCache.Insert(item);
            }

            observableCollection[index] = item;
        }

        public bool Remove(T item)
        {
            bool removed;
            if (removed = observableCollection.Remove(item))
            {
                if (ElementObserved)
                {
                    propertyProxyCache.Remove(item);
                }
            }

            return removed;
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (ElementObserved)
            {
                return observableEnumeratorFactory(observableCollection, propertyProxyCache);
            }

            return new Enumerator<T>(observableCollection);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Subscribe(IPropertyAccessedSubscriber subscriber)
        {
            propertyProxyCache?.SubscribeAll(subscriber);
        }
    }

    internal class Enumerator<TElement> : IEnumerator<TElement>, IEnumerator
    {
        private readonly IObservableCollection<TElement> observableCollection;

        private int index = -1;

        public Enumerator(
            IObservableCollection<TElement> observableCollection)
        {
            this.observableCollection = observableCollection;

        }

        public virtual TElement Current
        {
            get
            {
                return observableCollection[index];
            }
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            return ++index < observableCollection.Count;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {
            // NOOP
        }
    }

    internal class ObservableEnumerator<TObservable> : Enumerator<TObservable>
        where TObservable : class
    {
        private readonly IPropertyProxyCache<TObservable> propertyProxyCache;


        public ObservableEnumerator(
            IObservableCollection<TObservable> observableCollection,
            IPropertyProxyCache<TObservable> propertyProxyCache) : base(observableCollection)
        {
            this.propertyProxyCache = propertyProxyCache;

        }

        public override TObservable Current
        {
            get
            {
                TObservable originalValue = base.Current;
                if (!propertyProxyCache.TryGetValue(originalValue, out TObservable boxedItem))
                {
                    boxedItem = propertyProxyCache.Insert(originalValue);
                }

                return boxedItem;
            }
        }
    }
}
