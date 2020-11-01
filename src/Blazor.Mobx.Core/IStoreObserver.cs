using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx
{
    public interface IStoreObserver<TStore>
        where TStore : class
    {
        void ExecuteInAction(Action action);

        Task ExecuteInActionAsync(Func<Task> action);

        void Autorun(Func<TStore, ValueTask> action);

        void Autorun(Action<TStore> action);
    }
}
