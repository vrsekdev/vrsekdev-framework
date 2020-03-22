using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.Reactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx
{
    internal class StoreHolder<TStore> : IStoreHolder<TStore>
        where TStore : class
    {
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

        private readonly IObservableFactory observableFactory;
        private readonly ILookup<PropertyInfo, ReactionWrapper<TStore>> reactionsLookup;

        public IObservableProperty RootObservableProperty { get; }

        public event EventHandler<ObservablePropertyStateChangedEventArgs> StatePropertyChangedEvent;
        public event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        public StoreHolder(
            IStoreMetadata<TStore> storeMetadata,
            IObservableFactoryFactory observableFactoryFactory)
        {
            this.observableFactory = observableFactoryFactory.Create(
                OnStatePropertyChanged,
                OnCollectionItemsChanged);

            reactionsLookup = storeMetadata.GetReactions()
                .SelectMany(action => action.ObservedProperties.Select(prop => new { PropertyInfo = prop, Action = action }))
                .ToLookup(x => x.PropertyInfo, x => x.Action);

            RootObservableProperty = CreateObservableProperty(typeof(TStore));
        }

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return observableFactory.CreateObservableProperty(type);
        }

        private void OnStatePropertyChanged(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            ExecuteWithWriteLock(() =>
            {
                foreach (var action in reactionsLookup[e.PropertyInfo])
                {
                    action.Invoke(RootObservableProperty);
                }

                StatePropertyChangedEvent?.Invoke(sender, e);
            });
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            ExecuteWithWriteLock(() =>
            {
                CollectionItemsChangedEvent?.Invoke(sender, e);
            });
        }

        private void ExecuteWithWriteLock(Action action)
        {
            if (!readerWriterLock.TryEnterWriteLock(0))
            {
                // Already being invoked. All changes are going to be rendered.
                // Possibly this call is from an invoked reaction
                return;
            }
            try
            {
                action();
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}
