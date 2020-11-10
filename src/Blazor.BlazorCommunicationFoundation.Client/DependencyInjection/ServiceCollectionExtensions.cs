using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.DependencyInjection;
using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Emit;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFClient(this IServiceCollection services)
        {
            services.AddBlazorCommunicationFoundation();

            services.AddTransient<RemoteMethodExecutor>();
        }

        public static void AddBCFContract<TInterface>(this IServiceCollection services)
            where TInterface : class
        {
            Type contractProxy = RuntimeProxyBuilder.BuildRuntimeType(typeof(TInterface));

            services.AddTransient<RuntimeProxy<TInterface>>();
            services.AddTransient(typeof(TInterface), contractProxy);
        }
    }
}
