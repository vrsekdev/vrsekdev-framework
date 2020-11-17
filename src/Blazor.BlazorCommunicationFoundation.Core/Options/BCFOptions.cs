using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.MessagePack;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options
{
    public class BCFOptions
    {
        public Type SerializerType { get; internal set; } = typeof(MessagePackInvocationSerializer);

        public Type ContractTypeSerializerType { get; internal set; } = typeof(AssemblyQualifiedNameHashContractTypeSerializer);
    }
}
