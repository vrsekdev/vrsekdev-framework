using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    internal class DelegateMethodInvoker : IMethodInvoker
    {
        private static MethodInfo convertTaskMethod = typeof(DelegateMethodInvoker).GetMethod(nameof(ConvertTask), BindingFlags.Instance | BindingFlags.NonPublic);

        private static Dictionary<Type, Func<Task, Task<object>>> delegateCache = new Dictionary<Type, Func<Task, Task<object>>>();

        public Task<object> InvokeAsync(MethodInfo methodInfo, object instance, object[] arguments)
        {
            Task taskResult;
            try
            {
                taskResult = (Task)methodInfo.Invoke(instance, arguments);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            if (taskResult.GetType() == typeof(Task))
            {
                // no return value
                return ConvertTaskNoResult(taskResult);
            }

            Type resultType = taskResult.GetType().GetGenericArguments()[0];
            if (!delegateCache.TryGetValue(resultType, out var cachedDelegate))
            {
                MethodInfo delegateMethod = convertTaskMethod.MakeGenericMethod(resultType);
                cachedDelegate = (Func<Task, Task<object>>)Delegate.CreateDelegate(typeof(Func<Task, Task<object>>), this, delegateMethod);
                delegateCache.Add(resultType, cachedDelegate);
            }

            return cachedDelegate(taskResult);
        }

        private async Task<object> ConvertTask<T>(Task task)
        {
            return await (Task<T>)task;
        }

        private async Task<object> ConvertTaskNoResult(Task task)
        {
            await task;
            return null;
        }
    }
}
