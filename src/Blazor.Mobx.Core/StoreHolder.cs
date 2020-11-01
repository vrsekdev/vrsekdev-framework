using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using VrsekDev.Blazor.Mobx.Abstractions.Utils;
using VrsekDev.Blazor.Mobx.DependencyInjection;
using VrsekDev.Blazor.Mobx.Reactables;
using VrsekDev.Blazor.Mobx.Reactables.Actions;
using VrsekDev.Blazor.Mobx.Reactables.Autoruns;
using VrsekDev.Blazor.Mobx.Reactables.ComputedValues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx
{
    internal class StoreHolder<TStore> : IStoreHolder<TStore>
        where TStore : class
    {
        private readonly Queue<ComputedValueChangedArgs> computedValueChangedQueue = new Queue<ComputedValueChangedArgs>();
        private readonly Queue<ObservablePropertyStateChangedArgs> propertyStateChangedQueue = new Queue<ObservablePropertyStateChangedArgs>();
        private readonly Queue<ObservableCollectionItemsChangedArgs> collectionChangedQueue =new Queue<ObservableCollectionItemsChangedArgs>();

        private readonly SemaphoreSlim transactionLock = new SemaphoreSlim(1, 1);
        private readonly IObservableFactory observableFactory;
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        public StoreSubscribers Subscribers { get; } = new StoreSubscribers();

        public IObservableProperty RootObservableProperty { get; }
        public IStoreDependencyInjector<TStore> DependencyInjector { get; }

        private readonly IStoreMetadata<TStore> storeMetadata;

        public MethodInterceptions StoreReactables { get; private set; } = new MethodInterceptions();

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
                OnPropertyStateChanged,
                OnCollectionItemsChanged);

            RootObservableProperty = CreateObservableProperty(typeof(TStore));
            InitializeStoreReactables();
        }

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return observableFactory.CreateObservableProperty(type);
        }

        public void ExecuteInTransaction(Action action)
        {
            if (!transactionLock.TryExecuteWithWriteLock(action))
            {
                throw new InvalidOperationException("Different transaction is already opened.");
            }

            DequeueAll();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            if (!(await transactionLock.TryExecuteWithWriteLockAsync(action)))
            {
                throw new InvalidOperationException("Different transaction is already opened.");
            }

            DequeueAll();
        }

        public void RegisterMethodAutorun(Func<TStore, ValueTask> autorunMethod)
        {
            IPropertyProxy propertyProxy = propertyProxyFactory.Create(RootObservableProperty);
            var store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);
            DependencyInjector.InjectDependency(store);

            IInvokableReactable target = new MethodAutorunContainer<TStore>(autorunMethod, store);
            ReactableInvoker<TStore> invoker = new ReactableInvoker<TStore>(target, this);
            invoker.PlantSubscriber(propertyProxy);
        }

        private void InitializeStoreReactables()
        {
            RegisterStoreAutoruns();

            List<MethodInterception> methodInterceptions = new List<MethodInterception>();
            methodInterceptions.AddRange(GetStoreComputedValueInterceptions());
            methodInterceptions.AddRange(GetStoreActionInterceptions());
            StoreReactables = new MethodInterceptions
            {
                Interceptions = methodInterceptions.ToArray()
            };
        }

        private void RegisterStoreAutoruns()
        {
            foreach (var autorunMethod in storeMetadata.GetAutoruns())
            {
                IPropertyProxy propertyProxy = propertyProxyFactory.Create(RootObservableProperty);
                var store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);
                DependencyInjector.InjectDependency(store);

                IInvokableReactable target = new StoreAutorunContainer<TStore>(autorunMethod, store);
                ReactableInvoker<TStore> invoker = new ReactableInvoker<TStore>(target, this);
                invoker.PlantSubscriber(propertyProxy);
            }
        }

        private IEnumerable<MethodInterception> GetStoreActionInterceptions()
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

        private IEnumerable<MethodInterception> GetStoreComputedValueInterceptions()
        {
            var computedValues = storeMetadata.GetComputedValues();
            return computedValues.Select(computedValueMethod =>
            {
                Type containerType = typeof(ComputedValueContainer<,>).MakeGenericType(typeof(TStore), computedValueMethod.ReturnType);
                IPropertyProxy propertyProxy = propertyProxyFactory.Create(RootObservableProperty);
                var store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);
                DependencyInjector.InjectDependency(store);

                IComputedValueInvokable target = (IComputedValueInvokable)Activator.CreateInstance(containerType, store);
                ComputedValueInvoker<TStore> invoker = new ComputedValueInvoker<TStore>(OnComputedValueChanged, target, this);
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

        private void OnComputedValueChanged(object sender, ComputedValueChangedArgs e)
        {
            bool executed = transactionLock.TryExecuteWithWriteLock(() =>
            {
                Subscribers.NotifyComputedValueChanged(e);
            });

            if (!executed)
            {
                computedValueChangedQueue.Enqueue(e);
                return;
            }

            DequeueAll();
        }

        private void OnPropertyStateChanged(object sender, ObservablePropertyStateChangedArgs e)
        {
            bool executed = transactionLock.TryExecuteWithWriteLock(() =>
            {
                Subscribers.NotifyPropertyStateChanged(e);
            });

            if (!executed)
            {
                propertyStateChangedQueue.Enqueue(e);
                return;
            }

            DequeueAll();
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedArgs e)
        {
            bool executed = transactionLock.TryExecuteWithWriteLock(() =>
            {
                Subscribers.NotifyCollectionItemsChanged(e);
            });

            if (!executed)
            {
                collectionChangedQueue.Enqueue(e);
                return;
            }

            DequeueAll();
        }

        private void DequeueAll()
        {
            var computedValuesBatch = DequeueComputedValues();
            var propertyBatch = DequeueProperties();
            var collectionBatch = DequeueCollections();

            if (propertyBatch.Count > 0 || collectionBatch.Count > 0 || computedValuesBatch.Count > 0)
            {
                Subscribers.NotifyBatchObservableChanged(new BatchObservableChangeArgs
                {
                    ComputedValueChanges = computedValuesBatch,
                    PropertyChanges = propertyBatch,
                    CollectionChanges = collectionBatch
                });
            }
        }

        private List<ComputedValueChangedArgs> DequeueComputedValues()
        {
            return Dequeue(computedValueChangedQueue, x => x.ComputedValue);

        }

        private List<ObservablePropertyStateChangedArgs> DequeueProperties()
        {
            return Dequeue(propertyStateChangedQueue, x => (x.ObservableProperty, x.PropertyInfo.Name));

        }

        private List<ObservableCollectionItemsChangedArgs> DequeueCollections()
        {
            return Dequeue(collectionChangedQueue, x => x);
        }

        private List<TArgs> Dequeue<TArgs, TKey>(Queue<TArgs> queue, Func<TArgs, TKey> getKeyFunc)
        {
            List<TArgs> batch = new List<TArgs>();

            if (queue.Count == 0)
                return batch;

            HashSet<TKey> hashset = new HashSet<TKey>();
            while (queue.TryDequeue(out TArgs item))
            {
                if (hashset.Add(getKeyFunc(item)))
                {
                    batch.Add(item);
                }
            }

            return batch;
        }
    }
}
