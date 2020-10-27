using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.PropertyObservers;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Components
{
    public abstract class BlazorMobxComponentBase : ComponentBase, IBlazorMobxComponent
    {
        [Inject]
        private IPropertyObserverFactory PropertyObserverFactory { get; set; }

        public virtual T CreateObservable<T>(T instance)
            where T : class
        {
            var observer = PropertyObserverFactory.Create<T>();
            observer.SetConsumer(this);
            observer.InitializeValues(instance);

            return observer.WrappedInstance;
        }

        public Task ForceUpdate()
        {
            return InvokeAsync(StateHasChanged);
        }
    }

    public abstract class BlazorMobxComponentBase<TStore> : BlazorMobxComponentBase
        where TStore : class
    {
        private Lazy<IStoreAccessor<TStore>> storeAccessor;

        public BlazorMobxComponentBase()
        {
            storeAccessor = new Lazy<IStoreAccessor<TStore>>(() =>
            {
                var storeAccessor = CascadeStoreAccessor ?? InjectedStoreAccessor ?? throw new ArgumentException("Store accessor is not available");
                storeAccessor.SetConsumer((IBlazorMobxComponent)this);

                return storeAccessor;
            });
        }

        protected TStore Store => storeAccessor.Value.Store;

        [Inject]
        private IStoreAccessor<TStore> InjectedStoreAccessor { get; set; }

        [CascadingParameter(Name = CascadeStoreHolder.CascadingParameterName)]
        private IStoreAccessor<TStore> CascadeStoreAccessor { get; set; }

        public void ResetStore()
        {
            storeAccessor.Value.ResetStore();
        }

        protected virtual T CreateObservable<T>()
            where T : class
        {
            return storeAccessor.Value.CreateObservable<T>();
        }

        public override T CreateObservable<T>(T instance)
        {
            return storeAccessor.Value.CreateObservable(instance);
        }
    }
}
