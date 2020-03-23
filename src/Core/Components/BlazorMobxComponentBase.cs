using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.Extensions;
using Havit.Blazor.Mobx.StoreAccessors;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Components
{
    public abstract class BlazorMobxComponentBase : ComponentBase, IBlazorMobxComponent
    {
        private bool isRendered;

        public async override Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            isRendered = false;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            isRendered = true;
            base.OnAfterRender(firstRender);
        }

        public bool IsRendered()
        {
            return isRendered;
        }

        public Task ForceUpdate()
        {
            return InvokeAsync(StateHasChanged);
        }
    }

    public abstract class BlazorMobxComponentBase<TStore> : BlazorMobxComponentBase
        where TStore : class
    {
        protected TStore Store => storeAccessor.Store;

        private IStoreAccessor<TStore> storeAccessor;

        [Inject]
        private IStoreAccessor<TStore> InjectedStoreAccessor { get; set; }

        [CascadingParameter(Name = CascadeStoreHolder.CascadingParameterName)]
        private IStoreAccessor<TStore> CascadeStoreAccessor { get; set; }

        protected override void OnParametersSet()
        {
            storeAccessor = CascadeStoreAccessor ?? InjectedStoreAccessor ?? throw new ArgumentException("Store accessor is not available");
            storeAccessor.SetConsumer((IBlazorMobxComponent)this);

            base.OnParametersSet();
        }

        public void ResetStore()
        {
            storeAccessor.ResetStore();
        }

        protected virtual T CreateObservable<T>()
            where T : class
        {
            return storeAccessor.CreateObservable<T>();
        }
    }
}
