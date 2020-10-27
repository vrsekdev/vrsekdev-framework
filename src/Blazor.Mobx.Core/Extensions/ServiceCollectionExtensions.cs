using VrsekDev.Blazor.Mobx.DependencyInjection;
using VrsekDev.Blazor.Mobx.Observables.Default.Extensions;
using VrsekDev.Blazor.Mobx.PropertyObservers;
using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace VrsekDev.Blazor.Mobx.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static MobxStoreRegistration<TState> AddMobxStore<TState>(this IServiceCollection services)
            where TState : class
        {
            return new MobxStoreRegistration<TState>(services);
        }

        public static void AddDefaultMobxProperties(this IServiceCollection services)
        {
            services.UseDefaultMobxObservableProperties();
            services.UseMobxRuntimeProxy();

            services.AddTransient<IPropertyObserverFactory, PropertyObserverFactory>();
        }
    }
}
