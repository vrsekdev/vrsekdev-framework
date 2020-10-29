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
        event EventHandler<ObservablePropertyStateChangedEventArgs> PropertyStateChangedEvent;
        event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;
        event EventHandler<ComputedValueChangedEventArgs> ComputedValueChangedEvent;
        event EventHandler<BatchObservableChangeEventArgs> BatchObservableChangeEvent;

        IObservableProperty RootObservableProperty { get; }

        public void ExecuteInTransaction(Action action);

        public Task ExecuteInTransactionAsync(Func<Task> action);

        void RegisterMethodAutorun(Func<T, ValueTask> autorunMethod);
    }
}
