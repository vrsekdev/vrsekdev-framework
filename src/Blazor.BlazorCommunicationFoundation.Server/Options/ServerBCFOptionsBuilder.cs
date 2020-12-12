using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options
{
    public class ServerBCFOptionsBuilder : BCFOptionsBuilderBase, IServerOptionsBuilder
    {
        private readonly IServiceCollection services;

        private readonly HashSet<Type> invocationSerializerTypes = new HashSet<Type>();
        private ServerBCFOptions serverOptions;

        public ServerBCFOptionsBuilder(
            IServiceCollection services)
        {
            Contracts = new ServerBCFContractCollection(services);
            this.services = services;
        }

        public IServerContractCollection Contracts { get; }


        public void AddSerializer(Type serializerType)
        {
            invocationSerializerTypes.Add(serializerType);
        }

        IServiceCollection IServerOptionsBuilder.GetServiceCollection()
        {
            return services;
        }

        public override BCFOptions Build()
        {
            if (serverOptions == null)
            {
                return Build();
            }

            return serverOptions;
        }

        ServerBCFOptions IOptionsBuilder<ServerBCFOptions>.Build()
        {
            if (serverOptions != null)
            {
                return serverOptions;
            }

            serverOptions = new ServerBCFOptions();
            Build(serverOptions);

            serverOptions.Contracts = Contracts;
            foreach (var serializerType in invocationSerializerTypes)
            {
                serverOptions.InvocationSerializerTypes.Add(serializerType);
            }

            return serverOptions;
        }
    }
}
