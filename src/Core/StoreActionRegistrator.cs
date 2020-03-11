using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    public abstract class StoreActionRegistrator<TStore>
    {
        internal List<ActionBuilder<TStore>> Builders = new List<ActionBuilder<TStore>>();

        public void Register(Func<ActionBuilderFactory<TStore>, ActionBuilder<TStore>> register)
        {
            var builder = register(new ActionBuilderFactory<TStore>());

            Builders.Add(builder);
        }

        public void Register(ActionBuilder<TStore> builder)
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
