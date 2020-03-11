using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Samples.Shared.Stores
{
    public class HomeStoreActions : StoreActionRegistrator<IHomeStore>
    {
        public override void Register()
        {
            Register(ActionBuilder.For<IHomeStore>(SomeAction)
                .Observe(x => x.Value));
        }

        public void SomeAction(IHomeStore store)
        {
            Console.WriteLine(store.Value);
        }
    }
}
