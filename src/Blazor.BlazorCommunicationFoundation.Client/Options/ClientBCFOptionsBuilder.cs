using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public class ClientBCFOptionsBuilder : BCFOptionsBuilder, IClientOptionsBuilder
    {
        public ClientBCFOptionsBuilder(IServiceCollection services)
        {
            Contracts = new ClientBCFContractCollection(services);
        }

        public IClientContractCollection Contracts { get; }

        ClientBCFOptions IOptionsBuilder<ClientBCFOptions>.Build()
        {
            ClientBCFOptions serverOptions = new ClientBCFOptions();
            serverOptions.Contracts = Contracts;

            return serverOptions;
        }
    }
}
