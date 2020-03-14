using Havit.Blazor.StateManagement.Mobx.Extensions;
using Havit.Blazor.StateManagement.Mobx.StoreAccessors;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Components
{
    public abstract class BlazorMobxComponentBase : ComponentBase
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

        /*private IParameterWrapper[] parameters;

        public BlazorMobxComponentBase()
        {
            parameters = GetType().GetProperties()
                .Where(x => x.IsParameterProperty())
                .Select(x => (IParameterWrapper)Activator.CreateInstance(typeof(ParameterWrapper<>).MakeGenericType(x.PropertyType), x, this))
                .ToArray();
        }

        protected override void OnParametersSet()
        {
            foreach (var parameter in parameters)
            {
                object parameterValue = parameter.GetValue(this);

                if (parameterValue is IPropertyObservable dynamicState)
                {
                    parameter.SetValue(this, dynamicState.ObservableProperty);
                }
            }

            base.OnParametersSet();
        }

        internal virtual IPropertyObservable PlantComponentsObservers(ObservableProperty observableProperty)
        {
            // Empty
            return DynamicStateProperty.Create(observableProperty);
        }

        private interface IParameterWrapper
        {
            Type ParameterType { get; }

            object GetValue(object instance);

            void SetValue(object instance, ObservableProperty observableProperty);
        }

        private class ParameterWrapper<T> : IParameterWrapper
            where T : class
        {
            private readonly BlazorMobxComponentBase component;

            private readonly Action<object, T> setValue;
            private readonly Func<object, T> getValue;

            public Type ParameterType { get; }

            public ParameterWrapper(PropertyInfo propertyInfo, BlazorMobxComponentBase component)
            {
                ParameterType = propertyInfo.PropertyType;

                setValue = (object instance, T value) => propertyInfo.SetValue(instance, value);
                getValue = (object instance) => (T)propertyInfo.GetValue(instance);
                this.component = component;
            }

            public object GetValue(object instance)
            {
                return getValue(instance);
            }

            public void SetValue(object instance, ObservableProperty observableProperty)
            {
                IPropertyObservable dynamicState = component.PlantComponentsObservers(observableProperty);

                setValue(instance, DynamicStateProperty.Box<T>(dynamicState));
            }
        }*/
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
            storeAccessor.SetConsumer(this);

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

        /*internal IPropertyObservable PlantComponentsObservers(ObservableProperty observableProperty)
        {
            IPropertyObservable dynamicState = storeAccessor.CreateObservable(observableProperty);
            if (storeAccessor is InjectedStoreAccessor<TStore> dynamicStoreAccessor)
            {
                dynamicStoreAccessor.SubscribeObserver(dynamicState);
            }

            return dynamicState;
        }*/
    }
}
