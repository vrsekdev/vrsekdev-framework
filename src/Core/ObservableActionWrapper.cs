using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObservableActionWrapper<TStore>
        where TStore : class
    {
        private readonly Dictionary<IObservableProperty, TStore> storeInstanceCache = new Dictionary<IObservableProperty, TStore>();

        private readonly IPropertyObservableFactory propertyObservableFactory;
        private readonly IPropertyObservableWrapper propertyObservableWrapper;
        private readonly Action<TStore> action;

        public ObservableActionWrapper(
            IPropertyObservableFactory propertyObservableFactory,
            IPropertyObservableWrapper propertyObservableWrapper,
            ActionBuilder<TStore> actionBuilder)
        {
            this.propertyObservableFactory = propertyObservableFactory;
            this.propertyObservableWrapper = propertyObservableWrapper;

            this.action = actionBuilder.Action;
            this.ObservedProperties = actionBuilder.ObservedProperties;
            this.ObservedCollections = actionBuilder.ObservedCollections;
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
                IPropertyObservable propertyObservable = propertyObservableFactory.Create(observableProperty);
                store = propertyObservableWrapper.WrapPropertyObservable<TStore>(propertyObservable);
            }

            return store;
        }
    }
}
