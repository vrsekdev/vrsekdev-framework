using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class GlobalHttpClientResolver : IHttpClientResolver
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IContractScopeProvider contractScopeProvider;

        public GlobalHttpClientResolver(
            IServiceProvider serviceProvider,
            IContractScopeProvider contractScopeProvider)
        {
            this.serviceProvider = serviceProvider;
            this.contractScopeProvider = contractScopeProvider;
        }

        public HttpClient GetHttpClient(Type contractType)
        {
            IContractScope scope = contractScopeProvider.GetScope(contractType);

            IHttpClientResolver clientResolver = (IHttpClientResolver)serviceProvider.GetRequiredService(scope.HttpClientResolverType);
            return clientResolver.GetHttpClient(contractType);
        }
    }
}
