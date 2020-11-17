using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public class ClientBCFOptionsBuilder : BCFOptionsBuilder, IClientOptionsBuilder
    {
        private readonly IServiceCollection services;

        private readonly ClientBCFScopeBuilder globalScopeBuilder;

        private readonly List<ClientBCFScopeBuilder> childScopeBuilders = new List<ClientBCFScopeBuilder>();

        public ClientBCFOptionsBuilder(IServiceCollection services)
        {
            this.services = services;

            globalScopeBuilder = new ClientBCFScopeBuilder(services);
        }

        public IClientContractCollection Contracts => globalScopeBuilder.Contracts;

        public void CreateScope(Action<IClientScopeBuilder> scopeBuilderAction)
        {
            ClientBCFScopeBuilder scopeBuilder = new ClientBCFScopeBuilder(services);
            scopeBuilderAction(scopeBuilder);
            childScopeBuilders.Add(scopeBuilder);
        }

        public void UseHttpClientResolver<TResolver>() where TResolver : IHttpClientResolver
        {
            globalScopeBuilder.UseHttpClientResolver<TResolver>();
        }

        public void UseNamedHttpClient(string httpClientName)
        {
            globalScopeBuilder.UseNamedHttpClient(httpClientName);
        }

        ClientBCFOptions IOptionsBuilder<ClientBCFOptions>.Build()
        {
            ServiceProviderContractScopeProvider contractScopeProvider = new ServiceProviderContractScopeProvider();
            services.AddSingleton<IContractScopeProvider>(contractScopeProvider);

            IContractScope globalScope = globalScopeBuilder.Build();
            foreach (IContractScope scope in childScopeBuilders.Select(X => X.Build()).Union(new[] { globalScope }))
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

            ClientBCFOptions serverOptions = new ClientBCFOptions();
            serverOptions.HttpClientResolverType = globalScope.HttpClientResolverType ?? serverOptions.HttpClientResolverType;

            return serverOptions;
        }
    }
}
