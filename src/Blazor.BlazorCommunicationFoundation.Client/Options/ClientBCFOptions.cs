using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public class ClientBCFOptions : BCFOptions
    {
        public Type HttpClientResolverType { get; internal set; } = typeof(DefaultHttpClientResolver);

        public IContractScope[] Scopes { get; internal set; }
    }
}
