using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public interface IMethodBinder
    {
        MethodInfo BindMethod(Type contractType, string methodName, ArgumentBindingInfo[] argumentsBindingInfo);

        MethodInfo BindMethod(Type declaringType, string methodName, object[] args);

    }
}
