using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using VrsekDev.Blazor.Mobx.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx
{
    internal interface IStoreHolder<TStore> : IObservableHolder<TStore>
        where TStore : class
    {
        MethodInterceptions StoreReactables { get; }
        IStoreDependencyInjector<TStore> DependencyInjector { get; }

        IObservableProperty CreateObservableProperty(Type type);
    }
}
