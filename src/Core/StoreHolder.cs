using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Abstractions.Events;
using Havit.Blazor.StateManagement.Mobx.Reactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class StoreHolder<TStore> : IStoreHolder<TStore>
        where TStore : class
    {
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
            foreach (var action in reactionsLookup[e.PropertyInfo])
            {
                action.Invoke(RootObservableProperty);
            }

            StatePropertyChangedEvent?.Invoke(sender, e);
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            CollectionItemsChangedEvent?.Invoke(sender, e);
        }
    }
}
