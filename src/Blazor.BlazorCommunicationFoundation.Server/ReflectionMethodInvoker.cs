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

        public Task<object> InvokeAsync(InvocationWrapper invocationWrapper)
        {
            Type interfaceType = Type.GetType(invocationWrapper.BindingInfo.TypeName);
            object instance = serviceProvider.GetRequiredService(interfaceType);

            MethodInfo method = BindMethod(instance.GetType(), invocationWrapper.BindingInfo);
            object result = method.Invoke(instance, invocationWrapper.Arguments);

            return (Task<object>)((dynamic)this).ConvertTask((dynamic)result);
        }

        private MethodInfo BindMethod(Type implementationType, BindingInfo bindingInfo)
        {
            return implementationType.GetMethod(bindingInfo.MethodName);
        }

        private async Task<object> ConvertTask<T>(Task<T> task)
        {
            return await task;
        }
    }
}
