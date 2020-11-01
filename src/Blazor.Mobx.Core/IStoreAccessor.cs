using VrsekDev.Blazor.Mobx.Abstractions.Components;
using VrsekDev.Blazor.Mobx.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx
{
    public interface IStoreAccessor<TStore> : IStoreObserver<TStore>
        where TStore : class
    {
        TStore Store { get; }

        void SetConsumer(IBlazorMobxComponent consumer);

        void SetConsumer(ComponentBase consumer);

        T CreateObservable<T>() where T : class;

        T CreateObservable<T>(T instance) where T : class;

        void ResetStore();
    }
}
