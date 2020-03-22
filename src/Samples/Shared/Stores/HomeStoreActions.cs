﻿using Havit.Blazor.Mobx.Reactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Samples.Shared.Stores
{
    public class HomeStoreActions : ReactionRegistrator<IHomeStore>
    {
        public override void Register()
        {
            Register(ReactionBuilder.For<IHomeStore>(SomeAction)
                .Observe(x => x.Value));
        }

        public void SomeAction(IHomeStore store)
        {
            Console.WriteLine(store.Value);
        }
    }
}
