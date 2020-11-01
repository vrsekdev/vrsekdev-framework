using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.Mobx.Reactables.Autoruns;
using VrsekDev.Blazor.Mobx.Reactables;

namespace VrsekDev.Blazor.Mobx.PropertyObservers
{
    internal class ObservablePropertyHolder<T> : IObservableHolder<T>
        where T : class
    {
        private readonly IPropertyProxyWrapper propertyProxyWrapper;
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IObservableFactory observableFactory;

        public StoreSubscribers Subscribers { get; } = new StoreSubscribers();

        public IObservableProperty RootObservableProperty { get; }

        public ObservablePropertyHolder(
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper,
            IObservableFactoryFactory observableFactoryFactory)
        {
            this.propertyProxyWrapper = propertyProxyWrapper;
            this.propertyProxyFactory = propertyProxyFactory;
            this.observableFactory = observableFactoryFactory.CreateFactory(
                OnPropertyStateChanged,
                OnCollectionItemsChanged);

            RootObservableProperty = observableFactory.CreateObservableProperty(typeof(T));
        }

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return observableFactory.CreateObservableProperty(type);
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
            IPropertyProxy propertyProxy = propertyProxyFactory.Create(RootObservableProperty);
            var store = propertyProxyWrapper.WrapPropertyObservable<T>(propertyProxy);

            IInvokableReactable target = new MethodAutorunContainer<T>(autorunMethod, store);
            ReactableInvoker<T> invoker = new ReactableInvoker<T>(target, this);
            invoker.PlantSubscriber(propertyProxy);
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
