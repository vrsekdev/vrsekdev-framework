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
        public StoreSubscribers Subscribers { get; } = new StoreSubscribers();

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

        public void RegisterMethodAutorun(Func<T, ValueTask> autorunMethod)
        {
            throw new NotImplementedException();
        }

        private void OnPropertyStateChanged(object sender, ObservablePropertyStateChangedArgs e)
        {
            Subscribers.NotifyPropertyStateChanged(e);
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedArgs e)
        {
            Subscribers.NotifyCollectionItemsChanged(e);
        }
    }
}
