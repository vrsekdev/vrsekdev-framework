using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFServer(this IServiceCollection services, Action<IServerOptionsBuilder> builderAction)
        {
            ServerBCFOptionsBuilder builder = new ServerBCFOptionsBuilder(services);
            builderAction(builder);
            BCFOptions options = builder.Build();
            ServerBCFOptions serverOptions = ((IServerOptionsBuilder)builder).Build();

            services.AddSingleton<IContractImplementationStore, ServiceProviderContractImplementationStore>(
                serviceProvider => new ServiceProviderContractImplementationStore(serverOptions.Contracts.Contracts, serviceProvider.GetRequiredService<IContractTypeSerializer>()));

            services.AddBlazorCommunicationFoundation(options);

            services.AddSingleton<IMethodInvoker, ReflectionMethodInvoker>();
            services.AddTransient<IAuthorizationContextProvider, AuthorizationContextProvider>();
            services.AddTransient<IAuthorizationHandler, AuthorizationHandler>();
            services.AddTransient<IContractImplementationResolver, ServiceProviderContractImplementationResolver>();
        }
    }
}
