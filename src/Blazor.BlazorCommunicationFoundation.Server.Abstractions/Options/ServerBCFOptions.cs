using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.MessagePack;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Options
{
    public class ServerBCFOptions : BCFOptions
    {
        public IServerContractCollection Contracts { get; internal set; }

        public override Type[] ContractTypes => Contracts.ContractTypes.ToArray();

        public HashSet<Type> InvocationSerializerTypes { get; } = new HashSet<Type>
        {
            typeof(MessagePackInvocationSerializer)
        };
    }
}
