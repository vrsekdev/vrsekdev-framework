using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Components
{
    public class BlazorMobxComponentBase<TStore> : ComponentBase
        where TStore : class
    {
        protected TStore Store => StoreAccessor.Store;

        [Inject]
        private IStoreAccessor<TStore> StoreAccessor { get; set; }

        public Task ForceUpdate()
        {
            return InvokeAsync(StateHasChanged);
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            StoreAccessor.SetConsumer(this);

            return base.SetParametersAsync(parameters);
        }

        public void ResetStore()
        {
            DynamicStateProperty.Unbox(Store).ObservableProperty.ResetValues();
        }

        protected T CreateObservable<T>()
            where T : class
        {
            return StoreAccessor.CreateObservable<T>();
        }
    }
}
