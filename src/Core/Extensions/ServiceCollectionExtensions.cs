using Havit.Blazor.Mobx.DependencyInjection;
using Havit.Blazor.Mobx.Observables.Default.Extensions;
using Havit.Blazor.Mobx.PropertyObservers;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Blazor.Mobx.Extensions
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

            services.AddTransient<IPropertyObserverFactory, PropertyObserverFactory>();
        }
    }
}
