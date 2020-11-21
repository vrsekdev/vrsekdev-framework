using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding
{
    public interface IContractBinder
    {
        Type BindContractType(string bindingIdentifier);

        MethodInfo BindContractMethod(string bindingIdentifier);

    }
}
