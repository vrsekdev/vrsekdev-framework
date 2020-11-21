using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.MessagePack;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Options
{
    public class BCFOptions
    {
        public Type SerializerType { get; internal set; } = typeof(MessagePackInvocationSerializer);

        public Type ContractTypeSerializerType { get; internal set; } = typeof(AssemblyQualifiedNameContractTypeSerializer);

        public Type ContractBinderSerializerType { get; internal set; } = typeof(SignatureHashContractBindingSerializer); 
    }
}
