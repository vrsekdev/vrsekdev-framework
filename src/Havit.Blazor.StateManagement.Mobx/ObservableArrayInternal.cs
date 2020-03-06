using Havit.Blazor.StateManagement.Mobx.Extensions;
using Havit.Blazor.StateManagement.Mobx.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    public abstract class ObservableArrayInternal : IEnumerable
    {
        public abstract IEnumerator GetEnumerator();

        public abstract void Add(object item);

        public abstract void AddRange(IEnumerable<object> items);
    }

    public class ObservableArrayInternal<T> : ObservableArrayInternal, IObservableArray<T>, IObservable
    {
        private readonly EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent;
        private readonly EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private readonly List<T> list;

        private bool ShouldObserveElements { get; }

        internal ObservableArrayInternal(
            EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent,
            EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent) : base()
        {
            this.statePropertyChangedEvent = statePropertyChangedEvent;
            this.collectionItemsChangedEvent = collectionItemsChangedEvent;
            list = new List<T>();

            ShouldObserveElements = typeof(T).HasObservableArrayElementAttribute();
        }

        public T this[int index]
        {
            get => list[index];
            set
            {
                T oldValue = this[index];

                var addedItems = new object[] { value };
                var removedItems = new object[] { oldValue };

                T item = value;
                TryBoxItem(ref item);

                list[index] = item;

                collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs(addedItems, removedItems)
                {
                    OldCount = Count,
                    NewCount = Count
                });
            }
        }

        public ObservableType ObservableType => ObservableType.Array;

        public int Count => list.Count;

        public bool IsReadOnly => ((IList)list).IsReadOnly;

        public void Add(T item)
        {
            TryBoxItem(ref item);

            var addedItems = new object[] { item };
            var removedItems = Enumerable.Empty<object>();

            int oldCount = Count;
            list.Add(item);

            collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs(addedItems, removedItems)
            {
                OldCount = oldCount,
                NewCount = Count
            });
        }

        public override void Add(object item)
        {
            Add((T)item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            int oldCount = Count;
            var removedItems = Enumerable.Empty<object>();

            foreach (T item in items)
            {
                T innerItem = item;
                TryBoxItem(ref innerItem);
                list.Add(innerItem);
            }

            collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs((IEnumerable<object>)items, removedItems)
            {
                OldCount = oldCount,
                NewCount = Count
            });
        }

        public override void AddRange(IEnumerable<object> items)
        {
            AddRange(items.Cast<T>());
        }

        public void Clear()
        {
            var addedItems = Enumerable.Empty<object>();
            IEnumerable<object> removedItems = (IEnumerable<object>)list;

            int oldCount = Count;

            list.Clear();

            collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs(addedItems, removedItems)
            {
                OldCount = oldCount,
                NewCount = Count
            });
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
            IEnumerable<object> removedItems = Enumerable.Empty<object>();

            bool removed;
            if (removed = list.Remove(item))
            {
                removedItems = new object[] { item };
            }

            collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs(addedItems, removedItems)
            {
                OldCount = oldCount,
                NewCount = Count
            });

            return removed;
        }

        private bool TryBoxItem(ref T item)
        {
            if (!ShouldObserveElements)
            {
                return false;
            }

            if (DynamicStateProperty.IsObservable(item))
            {
                return false;
            }

            ObservableProperty observableProperty = new ObservableProperty(typeof(T),
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
            observableProperty.OverwriteFrom(item);

            DynamicStateProperty dynamicState = DynamicStateProperty.Create(observableProperty);
            item = (T)DynamicStateProperty.Box(dynamicState, typeof(T));

            return true;
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
