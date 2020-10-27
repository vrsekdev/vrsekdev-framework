using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Stores
{
    public class AsyncMobxStore
    {
        public virtual IObservableCollection<Task> WaitingTasks { get; set; } = new List<Task>().ToObservableCollection();

        protected AsyncResult<TResult> CompleteAsync<TResult>(Task<TResult> task, TResult defaultVal = default)
        {
            WaitingTasks.Add(task);
            return new AsyncResult<TResult>(task, defaultVal);
        }
    }
}
