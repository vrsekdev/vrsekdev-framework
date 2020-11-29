using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Options
{
    public interface IServerOptionsBuilder : IOptionsBuilder<ServerBCFOptions>
    {
        IServerContractCollection Contracts { get; }

        internal IServiceCollection GetServiceCollection();
    }
}
