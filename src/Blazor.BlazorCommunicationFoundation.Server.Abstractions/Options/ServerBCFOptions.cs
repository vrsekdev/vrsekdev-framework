using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Options
{
    public class ServerBCFOptions : BCFOptions
    {
        public IServerContractCollection Contracts { get; internal set; }
    }
}
