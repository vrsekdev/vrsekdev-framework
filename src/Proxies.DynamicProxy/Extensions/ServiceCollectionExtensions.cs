using Havit.Blazor.Mobx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.DynamicProxy.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseMobxDynamicProxy(this IServiceCollection services)
        {
            services
                .AddTransient<IPropertyProxyFactory, DynamicProxyFactory>()
                .AddTransient<IPropertyProxyWrapper, DynamicProxyWrapper>();
        }
    }
}
