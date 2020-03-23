using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables.Reactions
{
    internal class ReactionWrapper<TStore>
        where TStore : class
    {
        private readonly Dictionary<IObservableProperty, TStore> storeInstanceCache = new Dictionary<IObservableProperty, TStore>();

        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;
        private readonly Action<TStore> action;

        public ReactionWrapper(
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper,
            ReactionBuilder<TStore> reactionBuilder)
        {
            this.propertyProxyFactory = propertyProxyFactory;
            this.propertyProxyWrapper = propertyProxyWrapper;

            this.action = reactionBuilder.Action;
            this.ObservedProperties = reactionBuilder.ObservedProperties;
            this.ObservedCollections = reactionBuilder.ObservedCollections;
        }

        public HashSet<PropertyInfo> ObservedProperties { get; }
        public HashSet<PropertyInfo> ObservedCollections { get; }

        public void Invoke(IObservableProperty observableProperty)
        {
            TStore store = GetStore(observableProperty);
            action(store);
        }

        private TStore GetStore(IObservableProperty observableProperty)
        {
            if (!storeInstanceCache.TryGetValue(observableProperty, out TStore store))
            {
                IPropertyProxy propertyProxy = propertyProxyFactory.Create(observableProperty);
                store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);
            }

            return store;
        }
    }
}
