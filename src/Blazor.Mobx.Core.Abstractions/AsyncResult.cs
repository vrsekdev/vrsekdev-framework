using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Abstractions
{
    internal interface IAsyncResult
    {
        Task UnderLyingTask { get; }
    }

    public class AsyncResult<T> : IAsyncResult
    {
        private readonly T defaultValue;

        public AsyncResult(
            Task<T> task,
            T defaultValue)
        {
            underlyingTask = task;
            this.defaultValue = defaultValue; 
        }

        private Task<T> underlyingTask;
        public Task UnderLyingTask => underlyingTask;

        public T Value
        {
            get
            {
                if (underlyingTask.IsCompleted)
                {
                    return underlyingTask.Result;
                }

                return defaultValue;
            }
        }

        internal void Wait()
        {
            underlyingTask.Wait();
        }

        public static implicit operator T(AsyncResult<T> asyncResult)
        {
            return asyncResult.Value;
        }
    }
}
