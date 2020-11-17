using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class NamedHttpClientResolver : IHttpClientResolver
    {
        private readonly NamedHttpClientNameContainer httpClientNameContainer;
        private readonly IHttpClientFactory httpClientFactory;

        public NamedHttpClientResolver(
            NamedHttpClientNameContainer httpClientNameContainer,
            IHttpClientFactory httpClientFactory)
        {
            this.httpClientNameContainer = httpClientNameContainer;
            this.httpClientFactory = httpClientFactory;
        }

        public HttpClient GetHttpClient()
        {
            return httpClientFactory.CreateClient(httpClientNameContainer.HttpClientName);
        }
    }
}
