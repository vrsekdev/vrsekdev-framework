using VrsekDev.Blazor.Mobx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseMobxRuntimeProxy(this IServiceCollection services)
        {
            services
                .AddTransient<IPropertyProxyFactory, RuntimeProxyFactory>()
                .AddTransient<IPropertyProxyWrapper, RuntimeProxyWrapper>();
        }
    }
}
