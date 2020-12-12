using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Options
{
    public abstract class BCFOptions
    {
        public Type TypeBindingSerializerType { get; internal set; } = typeof(SimpleNameContractTypeSerializer);

        public Type MethodBindingSerializerType { get; internal set; } = typeof(MethodSignatureContractMethodBindingSerializer);

        public abstract Type[] ContractTypes { get; }
    }
}
