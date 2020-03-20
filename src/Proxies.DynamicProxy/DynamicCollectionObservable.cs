using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.DynamicProxy
{
    internal abstract class DynamicCollectionObservable : IObservableCollection, IDisposable
    {
        internal abstract IObservableCollection ObservableCollection { get; }

        public abstract bool ElementObserved { get; }
        public abstract Type ElementType { get; }
        public abstract int CountElements { get; }

        internal abstract IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer);
        public abstract void Reset();
        public abstract void Dispose();
        public abstract IEnumerator GetEnumerator();
    }

    internal class DynamicCollectionObservable<T> : DynamicCollectionObservable, IObservableCollection<T>
    {
        private readonly IObservableCollection<T> observableArray;
        private readonly IObservableFactory observableFactory;

        private DynamicStatePropertyCache dynamicStatePropertyCache;

        public DynamicCollectionObservable(
            IObservableCollection<T> observableArray,
            IObservableFactory observableFactory)
        {
            this.observableArray = observableArray;
            this.observableFactory = observableFactory;
            ElementType = observableArray.ElementType;
            ElementObserved = observableArray.ElementObserved;
            if (ElementObserved)
            {
                dynamicStatePropertyCache = new DynamicStatePropertyCache(observableFactory);
            }
        }

        public T this[int index]
        {
            get
            {
                T originalValue = observableArray[index];
                if (!dynamicStatePropertyCache.TryGetValue(originalValue, out T boxedValue))
                {
                    return dynamicStatePropertyCache.Insert(originalValue);
                }

                return boxedValue;
            }
            set => Insert(index, value);
        }

        public int Count => observableArray.Count;
        public override int CountElements => Count;
        public bool IsReadOnly => ((IList)observableArray).IsReadOnly;
        public bool IsSynchronized => ((ICollection)observableArray).IsSynchronized;
        public object SyncRoot => ((ICollection)observableArray).SyncRoot;

        public override bool ElementObserved { get; }
        public override Type ElementType { get; }

        internal override IObservableCollection ObservableCollection => observableArray;

        public void Add(T item)
        {
            if (ElementObserved)
            {
                BoxItem(item);
            }

            observableArray.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (ElementObserved)
            {
                foreach (T item in items)
                {
                    BoxItem(item);
                }
            }

            observableArray.AddRange(items);
        }

        public void Clear()
        {
            Reset();

            observableArray.Clear();
        }

        public override void Reset()
        {
            if (ElementObserved)
            {
                dynamicStatePropertyCache.Dispose();
                dynamicStatePropertyCache = new DynamicStatePropertyCache(observableFactory);
            }
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var boxedItem in dynamicStatePropertyCache)
            {
                array[index] = boxedItem;
            }
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var boxedItem in dynamicStatePropertyCache)
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
                T previousOriginalValue = observableArray[index];
                if (dynamicStatePropertyCache.TryGetValue(previousOriginalValue, out T previousValue))
                {
                    dynamicStatePropertyCache.Remove(previousValue);
                }

                BoxItem(item);
            }

            observableArray[index] = item;
        }

        public bool Remove(T item)
        {
            bool removed;
            if (removed = observableArray.Remove(item))
            {
                if (ElementObserved)
                {
                    dynamicStatePropertyCache.Remove(item);
                }
            }

            return removed;
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        private T BoxItem(T originalItem)
        {
            return dynamicStatePropertyCache.Insert(originalItem);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>)GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            return new Enumerator(observableArray, dynamicStatePropertyCache);
        }

        public override void Dispose()
        {
            if (ElementObserved)
            {
                dynamicStatePropertyCache.Dispose();
                dynamicStatePropertyCache = null;
            }
        }

        internal override IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer)
        {
            return dynamicStatePropertyCache?.Subscribe(observer);
        }

        private class Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly IObservableCollection<T> observableCollection;
            private readonly DynamicStatePropertyCache dynamicStatePropertyCache;

            private int index = -1;

            public Enumerator(
                IObservableCollection<T> observableCollection,
                DynamicStatePropertyCache dynamicStatePropertyCache)
            {
                this.observableCollection = observableCollection;
                this.dynamicStatePropertyCache = dynamicStatePropertyCache;

            }

            public T Current
            {
                get
                {
                    T originalValue = observableCollection[index];
                    if (dynamicStatePropertyCache == null)
                    {
                        return originalValue;
                    }
                    
                    if (!dynamicStatePropertyCache.TryGetValue(originalValue, out T boxedItem))
                    {
                        boxedItem = dynamicStatePropertyCache.Insert(originalValue);
                    }

                    return boxedItem;
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

        private class DynamicStatePropertyCache : IEnumerable<T>, IDisposable
        {
            // TODO: Concurrent dictionary
            private readonly List<IObserver<PropertyAccessedArgs>> observers = new List<IObserver<PropertyAccessedArgs>>();
            private readonly Dictionary<T, T> boxedValuesDictionary = new Dictionary<T, T>();
            private readonly IObservableFactory observableFactory;

            public DynamicStatePropertyCache(
                IObservableFactory observableFactory)
            {
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
                bool removed;
                if (removed = boxedValuesDictionary.Remove(boxedValue))
                {
                    DynamicPropertyProxy dynamicProxy = DynamicPropertyProxy.Unbox((object)boxedValue);
                    dynamicProxy.Dispose();
                }

                return removed;
            }

            internal IDisposable Subscribe(
                IObserver<PropertyAccessedArgs> observer)
            {
                var disposer = new ObserverDisposer();

                foreach (var boxedValue in boxedValuesDictionary.Values)
                {
                    DynamicPropertyProxy dynamicProxy = DynamicPropertyProxy.Unbox((object)boxedValue);
                    if (dynamicProxy != null)
                    {
                        disposer.AddDisposeAction(dynamicProxy.Subscribe(observer));
                    }
                }
                disposer.AddDisposeAction(() => observers.Remove(observer));

                observers.Add(observer);
                return disposer;
            }

            private T BoxItem(T item)
            {
                IObservableProperty observableProperty;
                if (DynamicPropertyProxy.Unbox((object)item) is DynamicPropertyProxy dynamicState)
                {
                    observableProperty = dynamicState.ObservableProperty;
                }
                else
                {
                    observableProperty = observableFactory.CreateObservableProperty(typeof(T));
                    observableProperty.OverwriteFrom(item);
                }

                dynamicState = DynamicPropertyProxy.Create(observableProperty);
                foreach (var observer in observers)
                {
                    // TODO: Add disposer
                    dynamicState.Subscribe(observer);
                }

                return DynamicPropertyProxy.Box(dynamicState, typeof(T));
            }

            public void Dispose()
            {
                foreach (var boxedItem in boxedValuesDictionary.Values)
                {
                    DynamicPropertyProxy dynamicState = DynamicPropertyProxy.Unbox((object)boxedItem);
                    dynamicState.Dispose();
                }
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
}
