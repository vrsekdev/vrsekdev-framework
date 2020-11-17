using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options;
using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Emit;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFClient(this IServiceCollection services, Action<IClientOptionsBuilder> builderAction = null)
        {
            ClientBCFOptionsBuilder builder = new ClientBCFOptionsBuilder(services);
            builderAction ??= builder => { };
            builderAction(builder);

            services.AddBlazorCommunicationFoundation(builder.Build());

            services.AddTransient<RemoteMethodExecutor>();
        }
    }
}
