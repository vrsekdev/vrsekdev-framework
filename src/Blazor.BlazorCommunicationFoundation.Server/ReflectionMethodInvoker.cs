using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    internal class ReflectionMethodInvoker : IMethodInvoker
    {
        public Task<object> InvokeAsync(MethodInfo methodInfo, object instance, object[] arguments)
        {
            object result = methodInfo.Invoke(instance, arguments);
            if (result.GetType() == typeof(Task))
            {
                // no return value
                return ConvertTaskNoResult((Task)result);
            }

            return (Task<object>)GetType().GetMethod(nameof(ConvertTask), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(result.GetType().GetGenericArguments()[0])
                .Invoke(this, new object[] { result });
        }

        private MethodInfo BindMethod(Type implementationType, RequestBindingInfo bindingInfo)
        {
            return implementationType.GetMethod(bindingInfo.MethodName);
        }

        private async Task<object> ConvertTask<T>(Task<T> task)
        {
            return await task;
        }

        private async Task<object> ConvertTaskNoResult(Task task)
        {
            await task;
            return null;
        }
    }
}
