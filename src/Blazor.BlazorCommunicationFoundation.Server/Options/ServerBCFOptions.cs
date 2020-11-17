using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options
{
    public class ServerBCFOptions : BCFOptions
    {
        public IContractCollection Contracts { get; internal set; }
    }
}
