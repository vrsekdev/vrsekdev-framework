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

        public MobxStateRegistration(IServiceCollection services)
        {
            this.services = services;
        }

        public IServiceCollection AsSingleton()
        {
            services.AddTransient<IStateAccessor<TState>, DynamicStateAccessor<TState>>();
            services.AddSingleton<IStateHolder<TState>, StateHolder<TState>>();

            return services;
        }

        public IServiceCollection AsTransient()
        {
            services.AddTransient<IStateAccessor<TState>, DynamicStateAccessor<TState>>();
            services.AddTransient<IStateHolder<TState>, StateHolder<TState>>();

            return services;
        }
    }
}
