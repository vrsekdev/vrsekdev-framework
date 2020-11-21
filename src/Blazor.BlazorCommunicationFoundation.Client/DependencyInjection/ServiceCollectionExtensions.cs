﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFClient(this IServiceCollection services, Action<IClientOptionsBuilder> builderAction = null)
        {
            ClientBCFOptionsBuilder builder = new ClientBCFOptionsBuilder(services);
            builderAction ??= builder => { };
            builderAction(builder);
            BCFOptions options = builder.Build();
            ClientBCFOptions clientOptions = ((IClientOptionsBuilder)builder).Build();

            services.AddBlazorCommunicationFoundation(options);

            services.AddTransient<IHttpClientResolver, GlobalHttpClientResolver>();
            services.AddTransient<RemoteMethodExecutor>();

            ServiceProviderContractScopeProvider contractScopeProvider = new ServiceProviderContractScopeProvider();
            services.AddSingleton<IContractScopeProvider>(contractScopeProvider);

            foreach (IContractScope scope in clientOptions.Scopes)
            {
                foreach (Type contractType in scope.ContractTypes)
                {
                    contractScopeProvider.AddScope(contractType, scope);
                    if (!services.Any(x => x.ImplementationType == scope.HttpClientResolverType))
                    {
                        services.AddTransient(scope.HttpClientResolverType);
                    }
                }
            }
        }
    }
}
