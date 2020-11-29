using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions
{
    public interface IContractTypeBindingSerializer
    {
        string GenerateIdentifier(Type type);
    }
}
