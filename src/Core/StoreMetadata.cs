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
        public virtual ReactionWrapper<TStore>[] GetReactions()
        {
            // No class registered
            return new ReactionWrapper<TStore>[0];
        }
    }

    internal class StoreMetadata<TStore, TImpl> : StoreMetadata<TStore>
        where TStore : class
        where TImpl : ReactionRegistrator<TStore>
    {
        private readonly IPropertyObservableFactory propertyObservableFactory;
        private readonly IPropertyObservableWrapper propertyObservableWrapper;

        private ReactionWrapper<TStore>[] reactions;

        public StoreMetadata(
            IPropertyObservableFactory propertyObservableFactory,
            IPropertyObservableWrapper propertyObservableWrapper,
            TImpl reactionRegistrator)
        {
            this.propertyObservableFactory = propertyObservableFactory;
            this.propertyObservableWrapper = propertyObservableWrapper;

            Initialize(reactionRegistrator);
        }

        public override ReactionWrapper<TStore>[] GetReactions()
        {
            return reactions;
        }

        private void Initialize(TImpl reactionRegistrator)
        {
            reactionRegistrator.RegisterInternal();

            reactions = reactionRegistrator.Builders.Select(builder =>
                new ReactionWrapper<TStore>(
                    propertyObservableFactory,
                    propertyObservableWrapper,
                    builder
                )
            ).ToArray();
        }
    }
}
