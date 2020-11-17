using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public class AssemblyQualifiedNameContractTypeSerializer : IContractTypeSerializer
    {
        public string GenerateIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}
