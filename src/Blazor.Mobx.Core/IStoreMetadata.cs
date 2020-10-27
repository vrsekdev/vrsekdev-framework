using VrsekDev.Blazor.Mobx.Reactables.Reactions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.Mobx
{
    internal interface IStoreMetadata<TStore>
        where TStore : class
    {
        MethodInfo[] GetAutoruns();

        MethodInfo[] GetComputedValues();

        MethodInfo[] GetActions();

        ReactionWrapper<TStore>[] GetReactions();
    }
}
