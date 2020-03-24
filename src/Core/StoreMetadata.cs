using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Attributes;
using Havit.Blazor.Mobx.Extensions;
using Havit.Blazor.Mobx.Reactables.ComputedValues;
using Havit.Blazor.Mobx.Reactables.Reactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Havit.Blazor.Mobx
{
    internal class StoreMetadata<TStore> : IStoreMetadata<TStore>
        where TStore : class
    {
        public MethodInfo[] GetAutoruns()
        {
            return typeof(TStore).GetMethods()
                .Where(x => x.GetCustomAttribute<AutorunAttribute>() != null).ToArray();
        }

        public virtual MethodInfo[] GetActions()
        {
            return typeof(TStore).GetMethods()
                .Where(x => x.GetCustomAttribute<ActionAttribute>() != null).ToArray();
        }

        public virtual MethodInfo[] GetComputedValues()
        {
            return typeof(TStore).GetMethods()
                .Where(x => x.GetCustomAttribute<ComputedValueAttribute>() != null).ToArray();
        }

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
