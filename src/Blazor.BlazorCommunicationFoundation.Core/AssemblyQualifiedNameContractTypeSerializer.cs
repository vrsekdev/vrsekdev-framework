using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation
{
    public class AssemblyQualifiedNameContractTypeSerializer : IContractTypeSerializer
    {
        public string GenerateIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}
