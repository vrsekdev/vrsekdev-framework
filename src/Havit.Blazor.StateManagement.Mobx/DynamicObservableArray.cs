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

    }

    internal class DynamicObservableArray<T> : DynamicObservableArray, IObservableArray<T>, IDisposable
    {
        private readonly DynamicStatePropertyCache dynamicStatePropertyCache;
        private readonly ObservableArrayInternal<T> observableArray;
        private readonly ObservablePropertyFactory observablePropertyFactory;

        private bool ShouldObserveElements { get; }

        public DynamicObservableArray(
            ObservableArrayInternal<T> observableArray)
        {
            this.observableArray = observableArray;

            ShouldObserveElements = typeof(T).HasObservableArrayElementAttribute();
            if (ShouldObserveElements)
            {
                observablePropertyFactory = new ObservablePropertyFactory(
                    observableArray.statePropertyChangedEvent,
                    observableArray.collectionItemsChangedEvent);
                dynamicStatePropertyCache = new DynamicStatePropertyCache(observablePropertyFactory);
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
            set
            {
                if (ShouldObserveElements)
                {
                    T previousOriginalValue = observableArray[index];
                    if (dynamicStatePropertyCache.TryGetValue(previousOriginalValue, out T previousValue))
                    {
                        dynamicStatePropertyCache.Remove(previousValue);
                    }

                    BoxItem(value);
                }

                observableArray[index] = value;
            }
        }

        public override int Count => observableArray.Count;

        public bool IsReadOnly => ((IList)observableArray).IsReadOnly;

        public override bool IsSynchronized => ((ICollection)observableArray).IsSynchronized;

        public override object SyncRoot => ((ICollection)observableArray).SyncRoot;

        internal override Type ObservedElementType => observableArray.ObservedElementType;

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
            BoxItem(item);

            observableArray.Add(item, suppressEvent);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (ShouldObserveElements)
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
            if (ShouldObserveElements)
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
            if (ShouldObserveElements)
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
            throw new NotImplementedException();
        }

        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            bool removed;
            if (removed = observableArray.Remove(item))
            {
                if (ShouldObserveElements)
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
            if (ShouldObserveElements)
            {
                dynamicStatePropertyCache.Dispose();
            }
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
            private readonly Dictionary<T, T> boxedValuesDictionary = new Dictionary<T, T>();
            private readonly ObservablePropertyFactory observablePropertyFactory;

            public DynamicStatePropertyCache(
                ObservablePropertyFactory observablePropertyFactory)
            {
                this.observablePropertyFactory = observablePropertyFactory;
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

            private T BoxItem(T item)
            {
                ObservableProperty observableProperty;
                if (DynamicStateProperty.Unbox((object)item) is DynamicStateProperty dynamicState)
                {
                    observableProperty = dynamicState.ObservableProperty;
                }
                else
                {
                    observableProperty = observablePropertyFactory.Create(typeof(T));
                    observableProperty.OverwriteFrom(item);
                }

                dynamicState = DynamicStateProperty.Create(observableProperty);
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
