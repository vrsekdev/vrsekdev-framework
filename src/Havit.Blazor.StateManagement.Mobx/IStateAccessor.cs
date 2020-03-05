using Havit.Blazor.StateManagement.Mobx.Components;
using Microsoft.AspNetCore.Components;

namespace Havit.Blazor.StateManagement.Mobx
{
    public interface IStateAccessor<TState>
        where TState : class
    {
        TState State { get; }

        void SetConsumer(BlazorMobxComponentBase<TState> consumer);

        void SetConsumer(ComponentBase consumer);
    }
}
