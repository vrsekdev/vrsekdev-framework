using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObservableActionWrapper<TStore>
        where TStore : class
    {
        private readonly IPropertyObservableFactory propertyObservableFactory;
        private readonly IPropertyObservableWrapper propertyObservableWrapper;
        private readonly Action<TStore> action;

        public ObservableActionWrapper(
            ActionBuilder<TStore> actionBuilder,
            IPropertyObservableFactory propertyObservableFactory,
            IPropertyObservableWrapper propertyObservableWrapper)
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
            IPropertyObservable propertyObservable = propertyObservableFactory.Create(observableProperty);
            TStore store = propertyObservableWrapper.WrapPropertyObservable<TStore>(propertyObservable);
            action(store);
        }
    }
}
