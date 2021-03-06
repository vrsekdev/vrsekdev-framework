﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFClient(this IServiceCollection services, Action<IClientOptionsBuilder> builderAction)
        {
            IClientOptionsBuilder builder = new ClientBCFOptionsBuilder(services);
            builderAction(builder);
            ClientBCFOptions clientOptions = builder.Build();

            services.AddBlazorCommunicationFoundation(clientOptions);

            if (clientOptions.InvocationSerializerType == null)
            {
                throw new ArgumentNullException("Type is required", nameof(clientOptions.InvocationSerializerType));
            }
            services.AddSingleton(typeof(IInvocationSerializer), clientOptions.InvocationSerializerType);
            services.AddTransient<IHttpClientResolver, GlobalHttpClientResolver>();
            services.AddTransient<RemoteMethodExecutor>();

            ServiceProviderContractScopeProvider contractScopeProvider = new ServiceProviderContractScopeProvider();
            services.AddSingleton<IContractScopeProvider>(contractScopeProvider);

            HashSet<Type> contractTypes = new HashSet<Type>();
            foreach (IContractScope scope in clientOptions.Scopes)
            {
                foreach (Type contractType in scope.ContractTypes)
                {
                    contractTypes.Add(contractType);
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
