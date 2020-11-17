using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public class ClientBCFOptionsBuilder : BCFOptionsBuilder, IClientOptionsBuilder
    {
        private readonly IServiceCollection services;

        public ClientBCFOptionsBuilder(IServiceCollection services)
        {
            Contracts = new ClientBCFContractCollection(services);
            this.services = services;
        }

        public IClientContractCollection Contracts { get; }

        public Type HttpClientResolverType { get; set; }

        public void UseHttpClientResolver<TResolver>() where TResolver : IHttpClientResolver
        {
            HttpClientResolverType = typeof(TResolver);
        }

        public void UseNamedHttpClient(string httpClientName)
        {
            services.AddSingleton(new NamedHttpClientNameContainer(httpClientName));
            HttpClientResolverType = typeof(NamedHttpClientResolver);
        }

        ClientBCFOptions IOptionsBuilder<ClientBCFOptions>.Build()
        {
            ClientBCFOptions serverOptions = new ClientBCFOptions();
            serverOptions.Contracts = Contracts;
            serverOptions.HttpClientResolverType = HttpClientResolverType ?? serverOptions.HttpClientResolverType;

            return serverOptions;
        }
    }
}
