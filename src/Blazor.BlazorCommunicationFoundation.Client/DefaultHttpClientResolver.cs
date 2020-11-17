using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class DefaultHttpClientResolver : IHttpClientResolver
    {
        private readonly HttpClient httpClient;

        public DefaultHttpClientResolver(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public HttpClient GetHttpClient()
        {
            return httpClient;
        }
    }
}
