using Microsoft.Extensions.DependencyInjection;
using System;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options
{
    public class ServerBCFOptionsBuilder : BCFOptionsBuilder, IServerOptionsBuilder
    {
        private readonly IServiceCollection services;

        public ServerBCFOptionsBuilder(
            IServiceCollection services)
        {
            Contracts = new ServerBCFContractCollection(services);
            this.services = services;
        }

        public IServerContractCollection Contracts { get; }

        ServerBCFOptions IOptionsBuilder<ServerBCFOptions>.Build()
        {
            ServerBCFOptions serverOptions = new ServerBCFOptions();
            serverOptions.Contracts = Contracts;

            return serverOptions;
        }

        IServiceCollection IServerOptionsBuilder.GetServiceCollection()
        {
            return services;
        }
    }
}
