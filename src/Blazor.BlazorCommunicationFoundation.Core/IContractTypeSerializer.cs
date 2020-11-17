using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public interface IContractTypeSerializer
    {
        string GenerateIdentifier(Type type);
    }
}
