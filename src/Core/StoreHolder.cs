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
        private readonly Queue<ComputedValueChangedEventArgs> computedValueChangedQueue = new Queue<ComputedValueChangedEventArgs>();
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

        public event EventHandler<ComputedValueChangedEventArgs> ComputedValueChangedEvent;
        public event EventHandler<ObservablePropertyStateChangedEventArgs> PropertyStateChangedEvent;
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
                OnPropertyStateChanged,
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

        public IObservableProperty CreateObservableProperty(Type type)
        {
            return observableFactory.CreateObservableProperty(type);
        }

        private void OnComputedValueChanged(object sender, ComputedValueChangedEventArgs e)
        {
            bool executed = transactionLock.TryExecuteWithWriteLock(() =>
            {
                ComputedValueChangedEvent?.Invoke(sender, e);
            });

            if (!executed)
            {
                computedValueChangedQueue.Enqueue(e);
                return;
            }

            DequeueAll();
        }

        private void OnPropertyStateChanged(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            bool executed = transactionLock.TryExecuteWithWriteLock(() =>
            {
                PropertyStateChangedEvent?.Invoke(sender, e);
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

        private void DequeueAll()
        {
            var computedValuesBatch = DequeueComputedValues();
            var propertyBatch = DequeueProperties();
            var collectionBatch = DequeueCollections();

            if (propertyBatch.Count > 0 || collectionBatch.Count > 0 || computedValuesBatch.Count > 0)
            {
                BatchObservableChangeEvent?.Invoke(this, new BatchObservableChangeEventArgs
                {
                    ComputedValueChanges = computedValuesBatch,
                    PropertyChanges = propertyBatch,
                    CollectionChanges = collectionBatch
                });
            }
        }

        private List<ComputedValueChangedEventArgs> DequeueComputedValues()
        {
            return Dequeue(computedValueChangedQueue, x => x.ComputedValue);

        }

        private List<ObservablePropertyStateChangedEventArgs> DequeueProperties()
        {
            return Dequeue(propertyStateChangedQueue, x => (x.ObservableProperty, x.PropertyInfo.Name));

        }

        private List<ObservableCollectionItemsChangedEventArgs> DequeueCollections()
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
