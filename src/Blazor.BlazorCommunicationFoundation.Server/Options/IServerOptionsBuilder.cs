using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options
{
    public interface IServerOptionsBuilder : IOptionsBuilder<ServerBCFOptions>
    {
        IServerContractCollection Contracts { get; }
    }
}
