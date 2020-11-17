using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public class ClientBCFScopeBuilder : IClientScopeBuilder
    {
        private object httpClientArgs;
        public Type httpClientResolverType;

        public ClientBCFScopeBuilder(
            IServiceCollection services)
        {
            Contracts = new ClientBCFContractCollection(services);
            httpClientResolverType = new ClientBCFOptions().HttpClientResolverType;
        }

        public IClientContractCollection Contracts { get; }

        public void UseHttpClientResolver<TResolver>() where TResolver : IHttpClientResolver
        {
            httpClientResolverType = typeof(TResolver);
        }

        public void UseNamedHttpClient(string httpClientName)
        {
            httpClientArgs = httpClientName;
            httpClientResolverType = typeof(NamedHttpClientResolver);
        }

        public IContractScope Build()
        {
            if (httpClientResolverType == null)
            {
                throw new ArgumentNullException("HttpClientResolverType type is required", nameof(httpClientResolverType));
            }

            ClientBCFContractScope scope = new ClientBCFContractScope();
            scope.ContractTypes = Contracts.ContractTypes;
            scope.HttpClientArgs = httpClientArgs;
            scope.HttpClientResolverType = httpClientResolverType;

            return scope;
        }
    }
}
