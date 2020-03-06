using Havit.Blazor.StateManagement.Mobx.Components;
using Microsoft.AspNetCore.Components;
using System;

namespace Havit.Blazor.StateManagement.Mobx
{
    public interface IStoreAccessor<TStore> : IDisposable
        where TStore : class
    {
        TStore Store { get; }

        void SetConsumer(BlazorMobxComponentBase<TStore> consumer);

        void SetConsumer(ComponentBase consumer);

        T CreateObservable<T>()
            where T : class;
    }
}
