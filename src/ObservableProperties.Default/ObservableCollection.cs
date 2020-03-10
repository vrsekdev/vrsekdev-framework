using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Abstractions.Events;
using Havit.Blazor.StateManagement.Mobx.ObservableProperties.Default.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.ObservableProperties.Default
{
    internal class ObservableCollection<T> : IObservableCollection<T>
    {
        private readonly EventHandler<ObservablePropertyStateChangedEventArgs> statePropertyChangedEvent;
        private readonly EventHandler<ObservableCollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private List<T> list;

        public bool ElementObserved { get; }
        public Type ElementType { get; }
        public int CountElements => Count;
        public int Count => list.Count;
        public bool IsReadOnly => ((IList)list).IsReadOnly;
        public bool IsSynchronized => ((ICollection)list).IsSynchronized;
        public object SyncRoot => ((ICollection)list).SyncRoot;

        public ObservableCollection(
            EventHandler<ObservablePropertyStateChangedEventArgs> statePropertyChangedEvent,
            EventHandler<ObservableCollectionItemsChangedEventArgs> collectionItemsChangedEvent) : base()
        {
            this.statePropertyChangedEvent = statePropertyChangedEvent;
            this.collectionItemsChangedEvent = collectionItemsChangedEvent;
            list = new List<T>();

            ElementType = typeof(T);
            ElementObserved = ElementType.HasObservableArrayElementAttribute();
        }

        public T this[int index]
        {
            get => list[index];
            set => Insert(index, value);
        }

        public void Add(T item)
        {
            // TODO: Box items into observable properties

            var addedItems = new object[] { item };
            var removedItems = Enumerable.Empty<object>();

            int oldCount = Count;
            list.Add(item);

            collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
            {
                ItemsAdded = addedItems,
                ItemsRemoved = removedItems,
                OldCount = oldCount,
                NewCount = Count
            });
        }

        public void AddRange(IEnumerable<T> items)
        {
            int oldCount = Count;
            var removedItems = Enumerable.Empty<object>();

            foreach (T item in items)
            {
                T innerItem = item;
                list.Add(innerItem);
            }

            collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
            {
                ItemsAdded = (IEnumerable<object>)items,
                ItemsRemoved = Enumerable.Empty<object>(),
                OldCount = oldCount,
                NewCount = Count
            });
        }

        public void Clear()
        {
            var addedItems = Enumerable.Empty<object>();
            IEnumerable<object> removedItems = (IEnumerable<object>)list;

            int oldCount = Count;

            list.Clear();

            collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
            {
                ItemsAdded = addedItems,
                ItemsRemoved = removedItems,
                OldCount = oldCount,
                NewCount = Count
            });
        }

        public void Reset()
        {
            Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            T oldValue = this[index];

            var addedItems = new object[] { item };
            var removedItems = new object[] { oldValue };

            list[index] = item;

            collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
            {
                ItemsAdded = addedItems,
                ItemsRemoved = removedItems,
                OldCount = Count,
                NewCount = Count
            });
        }

        public bool Remove(T item)
        {
            int oldCount = Count;
            IEnumerable<object> addedItems = Enumerable.Empty<object>();
            IEnumerable<object> removedItems = Enumerable.Empty<object>();

            bool removed;
            if (removed = list.Remove(item))
            {
                removedItems = new object[] { item };
            }

            collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
            {
                ItemsAdded = addedItems,
                ItemsRemoved = removedItems,
                OldCount = oldCount,
                NewCount = Count
            });

            return removed;
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void OverwriteElements(IEnumerable source)
        {
            list = new List<T>(source.Cast<T>());
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IList<T>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
}
