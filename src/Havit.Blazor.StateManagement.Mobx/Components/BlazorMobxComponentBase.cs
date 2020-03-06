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
        protected TStore Store => storeAccessor.Store;

        private IStoreAccessor<TStore> storeAccessor;

        [Inject]
        private IStoreAccessor<TStore> InjectedStoreAccessor { get; set; }

        [CascadingParameter(Name = CascadeStoreHolder.CascadingParameterName)]
        private IStoreAccessor<TStore> CascadeStoreAccessor { get; set; }

        public Task ForceUpdate()
        {
            return InvokeAsync(StateHasChanged);
        }

        protected override void OnParametersSet()
        {
            storeAccessor = CascadeStoreAccessor ?? InjectedStoreAccessor;
            storeAccessor.SetConsumer(this);

            base.OnParametersSet();
        }

        public void ResetStore()
        {
            DynamicStateProperty.Unbox(Store).ObservableProperty.ResetValues();
        }

        protected T CreateObservable<T>()
            where T : class
        {
            return InjectedStoreAccessor.CreateObservable<T>();
        }
    }
}
