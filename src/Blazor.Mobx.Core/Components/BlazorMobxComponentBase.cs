using VrsekDev.Blazor.Mobx.Abstractions.Components;
using VrsekDev.Blazor.Mobx.PropertyObservers;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using VrsekDev.Blazor.Mobx.Abstractions;
using System.Collections.Generic;

namespace VrsekDev.Blazor.Mobx.Components
{
    public abstract class BlazorMobxComponentBase : ComponentBase, IBlazorMobxComponent
    {
        private Dictionary<object, IStoreSubscriber> observableProperties = new Dictionary<object, IStoreSubscriber>();

        [Inject]
        private IPropertyObserverFactory PropertyObserverFactory { get; set; }

        protected T CreateObservable<T>() where T : class
        {
            var observer = PropertyObserverFactory.Create<T>();
            observer.SetConsumer(this);

            T observable = observer.WrappedInstance;
            observableProperties.Add(observable, observer);

            return observable;
        }

        public T CreateObservable<T>(T instance) where T : class
        {
            var observer = PropertyObserverFactory.Create<T>();
            observer.SetConsumer(this);
            observer.InitializeValues(instance);

            T observable = observer.WrappedInstance;
            observableProperties.Add(observable, observer);

            return observable;
        }

        public void ExecuteInAction<T>(T instance, Action action) where T : class
        {
            PropertyObserver<T> observer = GetObserverForProperty(instance);
            observer.ExecuteInAction(action);
        }

        public Task ExecuteInActionAsync<T>(T instance, Func<Task> action) where T : class
        {
            PropertyObserver<T> observer = GetObserverForProperty(instance);
            return observer.ExecuteInActionAsync(action);
        }

        public void Autorun<T>(T instance, Action<T> action) where T : class
        {
            PropertyObserver<T> observer = GetObserverForProperty(instance);
            observer.Autorun(action);
        }

        public void Autorun<T>(T instance, Func<T, ValueTask> action) where T : class
        {
            PropertyObserver<T> observer = GetObserverForProperty(instance);
            observer.Autorun(action);
        }

        public Task ForceUpdate()
        {
            return InvokeAsync(StateHasChanged);
        }

        private PropertyObserver<T> GetObserverForProperty<T>(T instance) where T : class
        {
            if (!observableProperties.TryGetValue(instance, out IStoreSubscriber storeSubscriber))
            {
                throw new ArgumentException("Instance is not a registered observable value");
            }

            return (PropertyObserver<T>)storeSubscriber;
        }
    }

    public abstract class BlazorMobxComponentBase<TStore> : BlazorMobxComponentBase
        where TStore : class
    {
        private Lazy<IStoreAccessor<TStore>> storeAccessor;

        protected BlazorMobxComponentBase()
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

        public void ExecuteInAction(Action action)
        {
            storeAccessor.Value.ExecuteInAction(action);
        }

        public Task ExecuteInActionAsync(Func<Task> action)
        {
            return storeAccessor.Value.ExecuteInActionAsync(action);
        }

        public void Autorun(Action<TStore> action)
        {
            storeAccessor.Value.Autorun(action);
        }

        public void Autorun(Func<TStore, ValueTask> action)
        {
            storeAccessor.Value.Autorun(action);
        }
    }
}
