using Havit.Blazor.Mobx.Reactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx
{
    internal interface IStoreMetadata<TStore>
        where TStore : class
    {
        ReactionWrapper<TStore>[] GetReactions();
    }
}
