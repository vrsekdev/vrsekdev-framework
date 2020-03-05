using Havit.Blazor.StateManagement.Mobx.Lifestyles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static MobxStateRegistration<TState> AddMobxState<TState>(this IServiceCollection services)
            where TState : class
        {
            return new MobxStateRegistration<TState>(services);
        }

    }
}
