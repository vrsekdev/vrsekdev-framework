using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    public interface IMethodInvoker
    {
        Task<object> InvokeAsync(MethodInfo methodInfo, object instance, object[] arguments);
    }
}
