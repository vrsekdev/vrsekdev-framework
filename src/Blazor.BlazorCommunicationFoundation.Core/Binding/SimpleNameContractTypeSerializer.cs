using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Binding
{
    public class SimpleNameContractTypeSerializer : IContractTypeBindingSerializer
    {
        public string GenerateIdentifier(Type type)
        {
            return type.Name;
        }
    }
}
