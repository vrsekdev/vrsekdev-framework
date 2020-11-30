using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Binding
{
    public interface IContractRequestPathHolder
    {
        string GetRequestPath(Type contractType, MethodInfo methodInfo);

        bool IsPathFamiliar(string path);
    }
}
