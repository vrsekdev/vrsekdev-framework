using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables.Reactions
{
    public abstract class ReactionRegistrator<TStore>
    {
        internal List<ReactionBuilder<TStore>> Builders = new List<ReactionBuilder<TStore>>();

        public void Register(Func<ReactionBuilderFactory<TStore>, ReactionBuilder<TStore>> register)
        {
            var builder = register(new ReactionBuilderFactory<TStore>());

            Builders.Add(builder);
        }

        public void Register(ReactionBuilder<TStore> builder)
        {
            Builders.Add(builder);
        }

        internal void RegisterInternal()
        {
            Register();
        }

        public abstract void Register();
    }
}
