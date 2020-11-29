using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.MessagePack;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Options
{
    public class BCFOptions
    {
        public ICollection<Type> InvocationSerializerTypes { get; } = new HashSet<Type>
        {
            typeof(MessagePackInvocationSerializer)
        };

        public Type ContractTypeSerializerType { get; internal set; } = typeof(SimpleNameContractTypeSerializer);

        public Type ContractBinderSerializerType { get; internal set; } = typeof(MethodSignatureContractMethodBindingSerializer); 
    }
}
