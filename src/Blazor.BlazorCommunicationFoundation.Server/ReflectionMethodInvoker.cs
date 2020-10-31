using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    internal class ReflectionMethodInvoker : IMethodInvoker
    {
        private readonly IServiceProvider serviceProvider;

        public ReflectionMethodInvoker(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task<object> InvokeAsync(MethodInfo methodInfo, object[] arguments)
        {
            object instance = serviceProvider.GetRequiredService(methodInfo.DeclaringType);

            object result = methodInfo.Invoke(instance, arguments);
            if (result.GetType() == typeof(Task))
            {
                // no return value
                return (Task<object>)GetType().GetMethod(nameof(ConvertTaskNoResult), BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(this, new object[] { result });
            }

            //return GetType().GetMethod(nameof(ConvertTask), BindingFlags.Instance | BindingFlags.NonPublic)
            //    .MakeGenericMethod(result.GetType().GetGenericArguments)
            return (Task<object>)((dynamic)this).ConvertTask((dynamic)result);
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
