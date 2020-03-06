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
        protected TStore Store { get; private set; }

        [Inject]
        private IStoreAccessor<TStore> StoreAccessor { get; set; }

        [CascadingParameter(Name = MobxStoreHolder.CascadingParameterName)]
        private TStore HierarchyStore { get; set; }

        public Task ForceUpdate()
        {
            return InvokeAsync(StateHasChanged);
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            StoreAccessor.SetConsumer(this);
            Store = HierarchyStore ?? StoreAccessor.Store;

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
