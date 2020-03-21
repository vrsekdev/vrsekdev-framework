using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Extensions;
using Havit.Blazor.StateManagement.Mobx.Reactions;
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
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        private ReactionWrapper<TStore>[] reactions;

        public StoreMetadata(
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper,
            TImpl reactionRegistrator)
        {
            this.propertyProxyFactory = propertyProxyFactory;
            this.propertyProxyWrapper = propertyProxyWrapper;

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
                    propertyProxyFactory,
                    propertyProxyWrapper,
                    builder
                )
            ).ToArray();
        }
    }
}
