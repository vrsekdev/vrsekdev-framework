using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding
{
    public interface IContractMethodBindingSerializer
    {
        string GenerateIdentifier(MethodInfo methodInfo);
    }
}
