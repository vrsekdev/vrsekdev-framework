using Havit.Blazor.StateManagement.Mobx.Lifestyles;
using Havit.Blazor.StateManagement.Mobx.Observables.Default.Extensions;
using Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Blazor.StateManagement.Mobx.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static MobxStoreRegistration<TState> AddMobxStore<TState>(this IServiceCollection services)
            where TState : class
        {
            return new MobxStoreRegistration<TState>(services);
        }

        public static void UseDefaultMobxProperties(this IServiceCollection services)
        {
            services.UseDefaultMobxObservableProperties();
            services.UseMobxRuntimeProxy();
        }
    }
}
