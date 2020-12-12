using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding
{
    public interface IContractTypeBindingSerializer
    {
        string GenerateIdentifier(Type type);
    }
}
