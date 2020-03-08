using Havit.Blazor.StateManagement.Mobx.Extensions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Components
{
    public class BlazorMobxComponentBase : ComponentBase
    {
        private IParameterWrapper[] parameters;

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
                object parametrValue = parameter.GetValue(this);

                if (DynamicStateProperty.Unbox(parametrValue) is DynamicStateProperty dynamicState)
                {
                    parameter.SetValue(this, dynamicState.ObservableProperty);
                }
            }

            base.OnParametersSet();
        }

        internal virtual DynamicStateProperty PlantComponentsObservers(ObservableProperty observableProperty)
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
                DynamicStateProperty dynamicState = component.PlantComponentsObservers(observableProperty);

                setValue(instance, DynamicStateProperty.Box<T>(dynamicState));
            }
        }
    }

    public class BlazorMobxComponentBase<TStore> : BlazorMobxComponentBase
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
            storeAccessor = CascadeStoreAccessor ?? InjectedStoreAccessor ?? throw new ArgumentException("Store accessor is not available");
            storeAccessor.SetConsumer(this);

            base.OnParametersSet();
        }

        public void ResetStore()
        {
            DynamicStateProperty.Unbox(Store).ObservableProperty.ResetValues();
        }

        protected virtual T CreateObservable<T>()
            where T : class
        {
            return storeAccessor.CreateObservable<T>();
        }

        internal override DynamicStateProperty PlantComponentsObservers(ObservableProperty observableProperty)
        {
            DynamicStateProperty dynamicState = DynamicStateProperty.Create(observableProperty);
            if (storeAccessor is DynamicStoreAccessor<TStore> dynamicStoreAccessor)
            {
                dynamicStoreAccessor.SubscribeObserver(dynamicState);

            }

            return dynamicState;
        }
    }
}
