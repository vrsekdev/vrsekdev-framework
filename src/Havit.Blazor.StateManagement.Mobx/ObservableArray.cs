using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    public abstract class ObservableArray : IEnumerable
    {
        public abstract IEnumerator GetEnumerator();
    }

    public class ObservableArray<T> : ObservableArray, IList<T>, IObservable
    {
        private readonly EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private readonly List<T> list;

        public ObservableArray(
            EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent)
        {
            this.collectionItemsChangedEvent = collectionItemsChangedEvent;
            list = new List<T>();
        }

        public ObservableArray(
            IEnumerable<object> elements,
            EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent)
        {
            this.collectionItemsChangedEvent = collectionItemsChangedEvent;
            list = new List<T>(elements.Cast<T>());
        }

        public T this[int index] { get => list[index]; set => list[index] = value; }

        public ObservableType ObservableType => ObservableType.Array;

        public int Count => list.Count;

        public bool IsReadOnly => ((IList)list).IsReadOnly;

        public void Add(T item)
        {
            var addedItems = new object[] { item };
            var removedItems = Enumerable.Empty<object>();

            collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs(addedItems, removedItems)
            {
                OldCount = Count,
                NewCount = Count + 1
            });

            list.Add(item);
        }

        public void Clear()
        {
            var addedItems = Enumerable.Empty<object>();
            IEnumerable<object> removedItems = list.Cast<object>();

            collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs(addedItems, removedItems)
            {
                OldCount = Count,
                NewCount = 0
            });

            list.Clear();
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
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            int oldCount = Count;
            IEnumerable<object> addedItems = Enumerable.Empty<object>();
            IEnumerable<object> removedItems;

            bool removed;
            if (removed = list.Remove(item))
            {
                removedItems = new object[] { item };

            }
            else
            {
                removedItems = Enumerable.Empty<object>();
            }

            collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs(addedItems, removedItems)
            {
                OldCount = oldCount,
                NewCount = Count
            });

            return removed;
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
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
