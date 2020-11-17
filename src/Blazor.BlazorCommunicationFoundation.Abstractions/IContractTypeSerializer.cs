using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions
{
    public interface IContractTypeSerializer
    {
        string GenerateIdentifier(Type type);
    }
}
