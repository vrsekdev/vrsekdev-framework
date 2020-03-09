using Havit.Blazor.StateManagement.Mobx.Extensions;
using Havit.Blazor.StateManagement.Mobx.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal abstract class DynamicObservableArray : ObservableArrayInternal
    {
        internal abstract ObservableArrayInternal ObservableArray { get; }

        internal abstract IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer);
    }

    internal class DynamicObservableArray<T> : DynamicObservableArray, IObservableArray<T>, IDisposable
    {
        private readonly DynamicStatePropertyCache dynamicStatePropertyCache;
        private readonly ObservableArrayInternal<T> observableArray;

        public DynamicObservableArray(
            ObservableArrayInternal<T> observableArray,
            ObservableFactory observableFactory)
        {
            this.observableArray = observableArray;
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

        public override int Count => observableArray.Count;
        public bool IsReadOnly => ((IList)observableArray).IsReadOnly;
        public override bool IsSynchronized => ((ICollection)observableArray).IsSynchronized;
        public override object SyncRoot => ((ICollection)observableArray).SyncRoot;

        internal override Type ElementType => observableArray.ElementType;
        internal override bool ElementObserved { get; }

        internal override ObservableArrayInternal ObservableArray => observableArray;

        public void Add(T item)
        {
            AddInternal(item, false);
        }

        internal override void Add(object item, bool suppressEvent)
        {
            AddInternal((T)item, suppressEvent);
        }

        internal void AddInternal(T item, bool suppressEvent)
        {
            if (ElementObserved)
            {
                BoxItem(item);
            }


            observableArray.Add(item, suppressEvent);
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

        internal override void AddRange(IEnumerable<object> items, bool suppressEvent = false)
        {
            if (ElementObserved)
            {
                foreach (T item in items)
                {
                    BoxItem(item);
                }
            }

            observableArray.AddRange(items, suppressEvent);
        }

        public void Clear()
        {
            if (ElementObserved)
            {
                foreach (var item in observableArray)
                {
                    DynamicStateProperty.Unbox(item).Dispose();
                }
            }

            observableArray.Clear();
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

        public override void CopyTo(Array array, int arrayIndex)
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

        public override IEnumerator<object> GetObjectEnumerator()
        {
            return (IEnumerator<object>)GetEnumerator();
        }

        public void Dispose()
        {
            if (ElementObserved)
            {
                dynamicStatePropertyCache.Dispose();
            }
        }

        internal override IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer)
        {
            return dynamicStatePropertyCache?.Subscribe(observer);
        }

        public override void OverwriteElements(IObservableArray elements)
        {
            //dynamicStatePropertyCache = new DynamicStatePropertyCache(ObservableArray);
        }

        private class Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly IList<T> list;
            private readonly DynamicStatePropertyCache dynamicStatePropertyCache;

            private readonly IEnumerator<T> thisEnumerator;

            private int index = -1;

            public Enumerator(
                IList<T> list,
                DynamicStatePropertyCache dynamicStatePropertyCache)
            {
                this.list = list;
                this.dynamicStatePropertyCache = dynamicStatePropertyCache;

                thisEnumerator = this;
            }

            public T Current
            {
                get
                {
                    T originalValue = list[index];
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

            object IEnumerator.Current => thisEnumerator.Current;

            public bool MoveNext()
            {
                return ++index < list.Count;
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
            private readonly ObservableFactory observableFactory;

            public DynamicStatePropertyCache(
                ObservableFactory observableFactory)
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
                    DynamicStateProperty dynamicState = DynamicStateProperty.Unbox((object)boxedValue);
                    dynamicState.Dispose();
                }

                return removed;
            }

            internal IDisposable Subscribe(
                IObserver<PropertyAccessedArgs> observer)
            {
                observers.Add(observer);
                var disposer = new ObserverDisposer();

                foreach (var boxedValue in boxedValuesDictionary.Values)
                {
                    DynamicStateProperty dynamicState = DynamicStateProperty.Unbox((object)boxedValue);
                    if (dynamicState != null)
                    {
                        disposer.AddDisposeAction(dynamicState.Subscribe(observer));
                    }
                }

                return disposer;
            }

            private T BoxItem(T item)
            {
                ObservableProperty observableProperty;
                if (DynamicStateProperty.Unbox((object)item) is DynamicStateProperty dynamicState)
                {
                    observableProperty = dynamicState.ObservableProperty;
                }
                else
                {
                    observableProperty = observableFactory.CreateObservableProperty(typeof(T));
                    observableProperty.OverwriteFrom(item);
                }

                dynamicState = DynamicStateProperty.Create(observableProperty);
                foreach (var observer in observers)
                {
                    // TODO: Add disposer
                    dynamicState.Subscribe(observer);
                }

                return DynamicStateProperty.Box(dynamicState, typeof(T));
            }

            public void Dispose()
            {
                foreach (var boxedItem in boxedValuesDictionary.Values)
                {
                    DynamicStateProperty dynamicState = DynamicStateProperty.Unbox((object)boxedItem);
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
