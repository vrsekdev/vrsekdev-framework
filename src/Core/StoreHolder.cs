using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.Reactables;
using Havit.Blazor.Mobx.Reactables.ComputedValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx
{
    internal class StoreHolder<TStore> : IStoreHolder<TStore>
        where TStore : class
    {
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly IObservableFactory observableFactory;
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        public IObservableProperty RootObservableProperty { get; }

        private readonly IStoreMetadata<TStore> storeMetadata;

        public MethodInterceptions StoreReactables { get; private set; } = new MethodInterceptions();

        public event EventHandler<ObservablePropertyStateChangedEventArgs> StatePropertyChangedEvent;
        public event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        public StoreHolder(
            IStoreMetadata<TStore> storeMetadata,
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper,
            IObservableFactoryFactory observableFactoryFactory)
        {
            this.storeMetadata = storeMetadata;
            this.propertyProxyFactory = propertyProxyFactory;
            this.propertyProxyWrapper = propertyProxyWrapper;
            this.observableFactory = observableFactoryFactory.CreateFactory(
                OnStatePropertyChanged,
                OnCollectionItemsChanged);

            RootObservableProperty = CreateObservableProperty(typeof(TStore));
            InitializeReactables();
        }

        private void InitializeReactables()
        {
            List<MethodInterception> methodInterceptions = new List<MethodInterception>();
            methodInterceptions.AddRange(GetComputedValueInterceptions());
            StoreReactables = new MethodInterceptions
            {
                Interceptions = methodInterceptions.ToArray()
            };
        }

        private IEnumerable<MethodInterception> GetComputedValueInterceptions()
        {
            var computedValues = storeMetadata.GetComputedValues();
            return computedValues.Select(computedValueMethod =>
            {
                Type containerType = typeof(ComputedValueContainer<,>).MakeGenericType(typeof(TStore), computedValueMethod.ReturnType);
                IPropertyProxy propertyProxy = propertyProxyFactory.Create(RootObservableProperty);
                var store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);

                IInvokableReactable target = (IInvokableReactable)Activator.CreateInstance(containerType, store);
                ReactableInvoker<TStore> invoker = new ReactableInvoker<TStore>(target, this);
                invoker.PlantSubscriber(propertyProxy);

                MethodInfo interceptorMethod = containerType.GetMethod("OnMethodInvoke");
                return new ClassMethodInterception
                {
                    InterceptedMethod = computedValueMethod,
                    InterceptorMethod = interceptorMethod,
                    InterceptorTarget = target,
                    ProvideInterceptedTarget = false
                };
            });
        }

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return observableFactory.CreateObservableProperty(type);
        }

        private void OnStatePropertyChanged(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            ExecuteWithWriteLock(() =>
            {
                StatePropertyChangedEvent?.Invoke(sender, e);
            });
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            ExecuteWithWriteLock(() =>
            {
                CollectionItemsChangedEvent?.Invoke(sender, e);
            });
        }

        // TODO: move this to store accessor to allow reactables to be invoked multiple times, 
        // but component should be rendered just once
        private void ExecuteWithWriteLock(Action action)
        {
            if (!readerWriterLock.TryEnterWriteLock(0))
            {
                // Already being invoked. All changes are going to be rendered.
                // Possibly this call is from an invoked reaction
                return;
            }
            try
            {
                action();
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}
