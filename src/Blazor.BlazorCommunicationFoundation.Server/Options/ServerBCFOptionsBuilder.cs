using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options
{
    public class ServerBCFOptionsBuilder : BCFOptionsBuilder, IServerOptionsBuilder
    {
        public ServerBCFOptionsBuilder(
            IServiceCollection services)
        {
            Contracts = new BCFContractCollection(services);
        }

        public IContractCollection Contracts { get; }

        ServerBCFOptions IOptionsBuilder<ServerBCFOptions>.Build()
        {
            ServerBCFOptions serverOptions = new ServerBCFOptions();
            serverOptions.Contracts ??= Contracts;

            return serverOptions;
        }
    }
}
