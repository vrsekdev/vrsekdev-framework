using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.Observables.Default.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Observables.Default
{
    internal abstract class ObservableCollection : IObservableCollection
    {
        public abstract Type ElementType { get; }
        public abstract bool ElementObserved { get; }
        internal abstract int CountElements { get; }

        public abstract void OverwriteElements(IEnumerable source);
        public abstract void Reset();
        public abstract IEnumerator GetEnumerator();
    }

    internal class ObservableCollection<T> : ObservableCollection, IObservableCollection<T>
    {
        private readonly EventHandler<ObservablePropertyStateChangedEventArgs> statePropertyChangedEvent;
        private readonly EventHandler<ObservableCollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private List<T> list;

        public override bool ElementObserved { get; }
        public override Type ElementType { get; }
        public int Count => list.Count;
        public bool IsReadOnly => ((IList)list).IsReadOnly;
        internal override int CountElements => Count;

        public ObservableCollection(
            bool observeElement,
            EventHandler<ObservablePropertyStateChangedEventArgs> statePropertyChangedEvent,
            EventHandler<ObservableCollectionItemsChangedEventArgs> collectionItemsChangedEvent) : base()
        {
            this.statePropertyChangedEvent = statePropertyChangedEvent;
            this.collectionItemsChangedEvent = collectionItemsChangedEvent;
            list = new List<T>();

            ElementType = typeof(T);
            ElementObserved = observeElement;
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
                ObservableCollection = this,
                ItemsAdded = addedItems,
                ItemsRemoved = removedItems,
                OldCount = oldCount,
                NewCount = Count
            });
        }

        public void AddDefaultElements(IEnumerable<T> items)
        {
            AddRangeInternal(items);
        }

        public void AddRange(IEnumerable<T> items)
        {
            int oldCount = Count;
            var removedItems = Enumerable.Empty<object>();

            AddRangeInternal(items);

            collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
            {
                ObservableCollection = this,
                ItemsAdded = (IEnumerable<object>)items,
                ItemsRemoved = Enumerable.Empty<object>(),
                OldCount = oldCount,
                NewCount = Count
            });
        }

        private void AddRangeInternal(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                T innerItem = item;
                list.Add(innerItem);
            }
        }

        public void Clear()
        {
            var addedItems = Enumerable.Empty<object>();
            IEnumerable<object> removedItems = (IEnumerable<object>)list;

            int oldCount = Count;

            list.Clear();

            collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
            {
                ObservableCollection = this,
                ItemsAdded = addedItems,
                ItemsRemoved = removedItems,
                OldCount = oldCount,
                NewCount = Count
            });
        }

        public override void Reset()
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
                ObservableCollection = this,
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
                ObservableCollection = this,
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

        public override void OverwriteElements(IEnumerable source)
        {
            list = new List<T>(source.Cast<T>());
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
