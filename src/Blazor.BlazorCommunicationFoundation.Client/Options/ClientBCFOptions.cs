using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public class ClientBCFOptions : BCFOptions
    {
        public IClientContractCollection Contracts { get; internal set; }

        public Type HttpClientResolverType { get; internal set; } = typeof(DefaultHttpClientResolver);
    }
}
