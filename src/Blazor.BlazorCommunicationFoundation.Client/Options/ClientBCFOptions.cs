using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.MessagePack;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public class ClientBCFOptions : BCFOptions
    {
        public Type InvocationSerializerType { get; internal set; } = typeof(MessagePackInvocationSerializer);

        public Type HttpClientResolverType { get; internal set; } = typeof(DefaultHttpClientResolver);

        public IContractScope[] Scopes { get; internal set; }

        public override Type[] ContractTypes => Scopes.SelectMany(x => x.ContractTypes).ToArray();
    }
}
