using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions
{
    public interface IContractBindingSerializer
    {
        string GenerateIdentifier(Type contractType, MethodInfo methodInfo);
    }
}
