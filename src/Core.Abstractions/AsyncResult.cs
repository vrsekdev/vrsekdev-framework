using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Abstractions
{
    public class AsyncResult<T>
    {
        private Task<T> underlyingTask;
        private T defaultValue;

        public AsyncResult(
            Task<T> task,
            T defaultValue)
        {
            underlyingTask = task;
            this.defaultValue = defaultValue; 
        }

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
    }
}
