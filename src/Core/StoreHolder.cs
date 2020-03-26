using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.Abstractions.Utils;
using Havit.Blazor.Mobx.DependencyInjection;
using Havit.Blazor.Mobx.Reactables;
using Havit.Blazor.Mobx.Reactables.Actions;
using Havit.Blazor.Mobx.Reactables.Autoruns;
using Havit.Blazor.Mobx.Reactables.ComputedValues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx
{
    internal class StoreHolder<TStore> : IStoreHolder<TStore>
        where TStore : class
    {
        private readonly Queue<ObservablePropertyStateChangedEventArgs> propertyStateChangedQueue = new Queue<ObservablePropertyStateChangedEventArgs>();
        private readonly Queue<ObservableCollectionItemsChangedEventArgs> collectionChangedQueue =new Queue<ObservableCollectionItemsChangedEventArgs>();

        private readonly ReaderWriterLockSlim transactionLock = new ReaderWriterLockSlim();
        private readonly IObservableFactory observableFactory;
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        public IObservableProperty RootObservableProperty { get; }
        public IStoreDependencyInjector<TStore> DependencyInjector { get; }

        private readonly IStoreMetadata<TStore> storeMetadata;

        public MethodInterceptions StoreReactables { get; private set; } = new MethodInterceptions();

        public event EventHandler<ObservablePropertyStateChangedEventArgs> StatePropertyChangedEvent;
        public event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;
        public event EventHandler<BatchObservableChangeEventArgs> BatchObservableChangeEvent;

        public StoreHolder(
            IStoreDependencyInjector<TStore> dependencyInjector,
            IStoreMetadata<TStore> storeMetadata,
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper,
            IObservableFactoryFactory observableFactoryFactory)
        {
            DependencyInjector = dependencyInjector;
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
            RegisterAutoruns();

            List<MethodInterception> methodInterceptions = new List<MethodInterception>();
            methodInterceptions.AddRange(GetComputedValueInterceptions());
            methodInterceptions.AddRange(GetActionInterceptions());
            StoreReactables = new MethodInterceptions
            {
                Interceptions = methodInterceptions.ToArray()
            };
        }

        private void RegisterAutoruns()
        {
            foreach (var autorunMethod in storeMetadata.GetAutoruns())
            {
                IPropertyProxy propertyProxy = propertyProxyFactory.Create(RootObservableProperty);
                var store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);
                DependencyInjector.InjectDependency(store);

                IInvokableReactable target = new AutorunContainer<TStore>(autorunMethod, store);
                ReactableInvoker<TStore> invoker = new ReactableInvoker<TStore>(target, this);
                invoker.PlantSubscriber(propertyProxy);
            }
        }

        private IEnumerable<MethodInterception> GetActionInterceptions()
        {
            var actions = storeMetadata.GetActions();
            return actions.Select(actionMethod =>
            {
                Contract.Requires(actionMethod.ReturnType == typeof(void));

                Delegate interceptor = ActionDelegate.GetFactoryForMethod(actionMethod)(transactionLock, DequeueAll);

                return new DelegateMethodInterception
                {
                    InterceptedMethod = actionMethod,
                    Delegate = interceptor
                };
            });
        }



        private IEnumerable<MethodInterception> GetComputedValueInterceptions()
        {
            var computedValues = storeMetadata.GetComputedValues();
            return computedValues.Select(computedValueMethod =>
            {
                Type containerType = typeof(ComputedValueContainer<,>).MakeGenericType(typeof(TStore), computedValueMethod.ReturnType);
                IPropertyProxy propertyProxy = propertyProxyFactory.Create(RootObservableProperty);
                var store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);
                DependencyInjector.InjectDependency(store);

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
            bool executed = transactionLock.TryExecuteWithWriteLock(() =>
            {
                StatePropertyChangedEvent?.Invoke(sender, e);
            });

            if (!executed)
            {
                propertyStateChangedQueue.Enqueue(e);
                return;
            }

            DequeueAll();
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            bool executed = transactionLock.TryExecuteWithWriteLock(() =>
            {
                CollectionItemsChangedEvent?.Invoke(sender, e);
            });

            if (!executed)
            {
                collectionChangedQueue.Enqueue(e);
                return;
            }

            DequeueAll();
        }

        private List<ObservablePropertyStateChangedEventArgs> DequeuProperties()
        {
            List<ObservablePropertyStateChangedEventArgs> batch = new List<ObservablePropertyStateChangedEventArgs>();

            if (propertyStateChangedQueue.Count == 0)
                return batch;

            HashSet<(IObservableProperty, string)> hashset = new HashSet<(IObservableProperty, string)>();
            while (propertyStateChangedQueue.TryDequeue(out ObservablePropertyStateChangedEventArgs item))
            {
                if (hashset.Add((item.ObservableProperty, item.PropertyInfo.Name)))
                {
                    batch.Add(item);
                }
            }

            return batch;
        }

        private void DequeueAll()
        {
            var propertyBatch = DequeuProperties();
            var collectionBatch = DequeueCollections();

            if (propertyBatch.Count > 0 || collectionBatch.Count > 0)
            {
                BatchObservableChangeEvent?.Invoke(this, new BatchObservableChangeEventArgs
                {
                    PropertyChanges = propertyBatch,
                    CollectionChanges = collectionBatch
                });
            }
        }

        private List<ObservableCollectionItemsChangedEventArgs> DequeueCollections()
        {
            List<ObservableCollectionItemsChangedEventArgs> batch = new List<ObservableCollectionItemsChangedEventArgs>();

            if (collectionChangedQueue.Count == 0)
                return batch;

            HashSet<ObservableCollectionItemsChangedEventArgs> hashset = new HashSet<ObservableCollectionItemsChangedEventArgs>();
            while (collectionChangedQueue.TryDequeue(out ObservableCollectionItemsChangedEventArgs item))
            {
                if (hashset.Add(item))
                {
                    // TODO: basically sending every change. Maybe group it?
                    batch.Add(item);
                }
            }

            return batch;
        }
    }
}
