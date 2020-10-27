using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx
{
    internal interface IStoreHolder<TStore> : IObservableHolder<TStore>
        where TStore : class
    {
        MethodInterceptions StoreReactables { get; }
        IStoreDependencyInjector<TStore> DependencyInjector { get; }

        IObservableProperty CreateObservableProperty(Type type);
    }
}
