using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding
{
    public interface IContractBinder
    {
        string GetRequestPath(Type contractType, MethodInfo methodInfo);

        bool IsPathFamiliar(string path);

        IReadOnlyDictionary<string, ContractMethodBinding> GetBindings();
    }
}
