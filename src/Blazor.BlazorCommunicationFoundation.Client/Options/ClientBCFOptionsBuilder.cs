using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public class ClientBCFOptionsBuilder : BCFOptionsBuilderBase, IClientOptionsBuilder
    {
        private readonly IServiceCollection services;

        private readonly ClientBCFScopeBuilder globalScopeBuilder;
        private readonly List<ClientBCFScopeBuilder> childScopeBuilders = new List<ClientBCFScopeBuilder>();

        private Type invocationSerializerType = null;

        public ClientBCFOptionsBuilder(IServiceCollection services)
        {
            this.services = services;

            globalScopeBuilder = new ClientBCFScopeBuilder(services);
        }

        public IClientContractCollection Contracts => globalScopeBuilder.Contracts;

        public void UseSerializer(Type type)
        {
            invocationSerializerType = type;
        }

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
            ClientBCFOptions clientOptions = new ClientBCFOptions();
            Build(clientOptions);

            IContractScope globalScope = globalScopeBuilder.Build();

            clientOptions.InvocationSerializerType = invocationSerializerType ?? clientOptions.InvocationSerializerType;
            clientOptions.HttpClientResolverType = globalScope.HttpClientResolverType ?? clientOptions.HttpClientResolverType;
            clientOptions.Scopes = childScopeBuilders.Select(X => X.Build()).Union(new[] { globalScope }).ToArray();

            return clientOptions;
        }

        public override BCFOptions Build()
        {
            return Build();
        }
    }
}
