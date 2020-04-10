using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.Components;
using Microsoft.AspNetCore.Components;
using System;

namespace Havit.Blazor.Mobx
{
    public interface IStoreAccessor<TStore>
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
