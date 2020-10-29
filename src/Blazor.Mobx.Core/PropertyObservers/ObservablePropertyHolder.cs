using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.PropertyObservers
{
    internal class ObservablePropertyHolder<T> : IObservableHolder<T>
        where T : class
    {
        public event EventHandler<ObservablePropertyStateChangedEventArgs> PropertyStateChangedEvent;
        public event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;
        public event EventHandler<ComputedValueChangedEventArgs> ComputedValueChangedEvent;
        public event EventHandler<BatchObservableChangeEventArgs> BatchObservableChangeEvent;

        public IObservableProperty RootObservableProperty { get; }

        public ObservablePropertyHolder(
            IObservableFactoryFactory observableFactoryFactory)
        {
            var observableFactory = observableFactoryFactory.CreateFactory(
                OnPropertyStateChanged,
                OnCollectionItemsChanged);

            RootObservableProperty = observableFactory.CreateObservableProperty(typeof(T));
        }

        public void ExecuteInTransaction(Action action)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteInTransactionAsync(Func<Task> action)
        {
            throw new NotImplementedException();
        }

        public void RegisterMethodAutorun(Action<T> autorunMethod)
        {
            throw new NotImplementedException();
        }

        private void OnPropertyStateChanged(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            PropertyStateChangedEvent?.Invoke(sender, e);
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            CollectionItemsChangedEvent?.Invoke(sender, e);
        }
    }
}
