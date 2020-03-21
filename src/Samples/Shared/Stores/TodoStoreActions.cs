using Havit.Blazor.StateManagement.Mobx.Reactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Samples.Shared.Stores
{
    public class TodoStoreActions : ReactionRegistrator<ITodoStore>
    {
        public override void Register()
        {
            Register(ReactionBuilder.For<ITodoStore>(SomeAction)
                .Observe(x => x.Value));
        }

        public void SomeAction(ITodoStore store)
        {
            Console.WriteLine(store.Value);
        }
    }
}
