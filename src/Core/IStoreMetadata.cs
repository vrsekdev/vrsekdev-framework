using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal interface IStoreMetadata<TStore>
        where TStore : class
    {
        ReactionWrapper<TStore>[] GetReactions();
    }
}
