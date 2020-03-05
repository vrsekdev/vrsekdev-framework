using Havit.Blazor.StateManagement.Mobx.Components;
using Microsoft.AspNetCore.Components;

namespace Havit.Blazor.StateManagement.Mobx
{
    public interface IStoreAccessor<TStore>
        where TStore : class
    {
        TStore Store { get; }

        void SetConsumer(BlazorMobxComponentBase<TStore> consumer);

        void SetConsumer(ComponentBase consumer);
    }
}
