using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    public interface IMethodInvoker
    {
        Task<object> InvokeAsync(MethodInfo method, object[] arguments);
    }
}
