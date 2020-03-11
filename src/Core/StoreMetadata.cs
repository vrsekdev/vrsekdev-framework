using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class StoreMetadata<TStore> : IStoreMetadata<TStore>
        where TStore : class
    {
        public virtual ObservableActionWrapper<TStore>[] GetObservableActions()
        {
            // No class registered
            return new ObservableActionWrapper<TStore>[0];
        }
    }

    internal class StoreMetadata<TStore, TImpl> : StoreMetadata<TStore>
        where TStore : class
        where TImpl : StoreActionRegistrator<TStore>
    {
        private readonly IPropertyObservableFactory propertyObservableFactory;
        private readonly IPropertyObservableWrapper propertyObservableWrapper;

        private ObservableActionWrapper<TStore>[] observableActions;

        public StoreMetadata(
            IPropertyObservableFactory propertyObservableFactory,
            IPropertyObservableWrapper propertyObservableWrapper,
            TImpl actionRegistrator)
        {
            this.propertyObservableFactory = propertyObservableFactory;
            this.propertyObservableWrapper = propertyObservableWrapper;

            Initialize(actionRegistrator);
        }

        public override ObservableActionWrapper<TStore>[] GetObservableActions()
        {
            return observableActions;
        }

        private void Initialize(TImpl actionRegistrator)
        {
            actionRegistrator.RegisterInternal();

            observableActions = actionRegistrator.Builders.Select(builder =>
                new ObservableActionWrapper<TStore>(
                    builder,
                    propertyObservableFactory,
                    propertyObservableWrapper
                )
            ).ToArray();
        }
    }
}
