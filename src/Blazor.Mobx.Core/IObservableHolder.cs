using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace VrsekDev.Blazor.Mobx
{
    internal interface IObservableHolder<T>
    {
        StoreSubscribers Subscribers { get; }

        IObservableProperty RootObservableProperty { get; }

        IObservableProperty CreateObservableProperty(Type type);

        public void ExecuteInTransaction(Action action);

        public Task ExecuteInTransactionAsync(Func<Task> action);

        void RegisterMethodAutorun(Func<T, ValueTask> autorunMethod);
    }
}
