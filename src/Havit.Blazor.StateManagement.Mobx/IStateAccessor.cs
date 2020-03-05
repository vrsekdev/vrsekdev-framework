using Havit.Blazor.StateManagement.Mobx.Components;

namespace Havit.Blazor.StateManagement.Mobx
{
    public interface IStateAccessor<TState>
        where TState : class
    {
        TState State { get; }

        void SetConsumer(BlazorMobxComponentBase<TState> consumer);
    }
}
