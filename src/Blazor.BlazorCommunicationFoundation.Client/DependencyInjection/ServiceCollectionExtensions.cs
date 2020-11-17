using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.DependencyInjection;
using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Emit;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFClient(this IServiceCollection services, Action<IOptionsBuilder<BCFOptions>> builderAction = null)
        {
            BCFOptionsBuilder builder = new BCFOptionsBuilder();
            builderAction ??= builder => { };
            builderAction(builder);

            services.AddBlazorCommunicationFoundation(builder.Build());

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
