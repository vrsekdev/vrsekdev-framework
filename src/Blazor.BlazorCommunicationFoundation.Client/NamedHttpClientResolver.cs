using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class NamedHttpClientResolver : IHttpClientResolver
    {
        private readonly IContractScopeProvider contractScopeProvider;
        private readonly IHttpClientFactory httpClientFactory;

        public NamedHttpClientResolver(
            IContractScopeProvider contractScopeProvider,
            IHttpClientFactory httpClientFactory)
        {
            this.contractScopeProvider = contractScopeProvider;
            this.httpClientFactory = httpClientFactory;
        }

        public HttpClient GetHttpClient(Type contractType)
        {
            IContractScope scope = contractScopeProvider.GetScope(contractType);

            return httpClientFactory.CreateClient((string)scope.HttpClientArgs);
        }
    }
}
