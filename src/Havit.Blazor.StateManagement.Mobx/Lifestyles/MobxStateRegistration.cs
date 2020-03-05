using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Lifestyles
{
    public class MobxStateRegistration<TState>
        where TState : class
    {
        private readonly IServiceCollection services;

        private TState defaultState;

        public MobxStateRegistration(IServiceCollection services)
        {
            this.services = services;
        }

        public IServiceCollection AsSingleton()
        {
            services.AddSingleton<IStateHolder<TState>>(GetStateHolder());
            services.AddTransient<IStateAccessor<TState>, DynamicStateAccessor<TState>>();

            return services;
        }

        public IServiceCollection AsTransient()
        {
            services.AddTransient<IStateHolder<TState>>(provider => GetStateHolder());
            services.AddTransient<IStateAccessor<TState>, DynamicStateAccessor<TState>>();

            return services;
        }

        private IStateHolder<TState> GetStateHolder()
        {
            var stateHolder = new StateHolder<TState>();
            if (defaultState != null)
            {
                stateHolder.RootObservableProperty.OverwriteFrom(defaultState);
            }

            return stateHolder;
        }

        public MobxStateRegistration<TState> WithDefaultState<TStateImpl>(TStateImpl defaultState)
            where TStateImpl : class, TState
        {
            this.defaultState = defaultState;

            return this;
        }
    }
}
