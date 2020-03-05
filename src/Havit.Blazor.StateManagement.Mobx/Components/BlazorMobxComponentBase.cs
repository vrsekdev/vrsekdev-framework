using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Components
{
    public class BlazorMobxComponentBase<TState> : ComponentBase
        where TState : class
    {
        protected TState State => StateAccessor.State;

        [Inject]
        private IStateAccessor<TState> StateAccessor { get; set; }

        public Task ForceUpdate()
        {
            return InvokeAsync(StateHasChanged);
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            StateAccessor.SetConsumer(this);

            return base.SetParametersAsync(parameters);
        }

        public void ResetState()
        {
            DynamicStateProperty.Unbox(State).ObservableProperty.ResetValues();
        }
    }
}
