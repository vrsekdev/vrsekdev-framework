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
        public Task ForceUpdate()
        {
            return InvokeAsync(StateHasChanged);
        }
    }

    public abstract class BlazorMobxComponentBase<TStore> : BlazorMobxComponentBase
        where TStore : class
    {
        private IStoreAccessor<TStore> storeAccessor;

        protected TStore Store
        {
            get
            {
                if (storeAccessor == null)
                {
                    storeAccessor = CascadeStoreAccessor ?? InjectedStoreAccessor ?? throw new ArgumentException("Store accessor is not available");
                    storeAccessor.SetConsumer((IBlazorMobxComponent)this);
                }

                return storeAccessor.Store;
            }
        }

        [Inject]
        private IStoreAccessor<TStore> InjectedStoreAccessor { get; set; }

        [CascadingParameter(Name = CascadeStoreHolder.CascadingParameterName)]
        private IStoreAccessor<TStore> CascadeStoreAccessor { get; set; }

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
