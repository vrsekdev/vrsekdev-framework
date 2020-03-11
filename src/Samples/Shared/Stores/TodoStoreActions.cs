using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Samples.Shared.Stores
{
    public class TodoStoreActions : StoreActionRegistrator<ITodoStore>
    {
        public override void Register()
        {
            Register(ActionBuilder.For<ITodoStore>(SomeAction)
                .Observe(x => x.Value));
        }

        public void SomeAction(ITodoStore store)
        {
            Console.WriteLine(store.Value);
        }
    }
}
