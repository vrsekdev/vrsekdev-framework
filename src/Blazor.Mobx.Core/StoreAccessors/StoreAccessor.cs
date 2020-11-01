using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Components;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.StoreAccessors
{
    internal class StoreAccessor<TStore> : ObserverBase<TStore>, IStoreAccessor<TStore>
        where TStore : class
    {
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        private IConsumerWrapper consumer;

        public StoreAccessor(
            IStoreHolder<TStore> storeHolder,
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper) : base(storeHolder)
        {
            this.propertyProxyWrapper = propertyProxyWrapper;

            IPropertyProxy propertyProxy = propertyProxyFactory.Create(storeHolder.RootObservableProperty, storeHolder.StoreReactables);
            Store = propertyProxyWrapper.WrapPropertyObservable<TStore>(propertyProxy);
            storeHolder.DependencyInjector.InjectDependency(Store);

            PlantSubscriber(propertyProxy);
        }

        public TStore Store { get; }

        public void SetConsumer(IBlazorMobxComponent consumer)
        {
            Contract.Requires(consumer == null);

            this.consumer = new MobxConsumerWrapper(consumer);
        }

        public void SetConsumer(ComponentBase consumer)
        {
            Contract.Requires(consumer == null);
            if (consumer is IBlazorMobxComponent mobxConsumer)
            {
                SetConsumer(mobxConsumer);
                return;
            }

            this.consumer = new ReflectionConsumerWrapper(consumer);
        }

        public void ResetStore()
        {
            propertyProxyWrapper.UnwrapPropertyObservable(Store).ObservableProperty.ResetValues();
        }

        private void PlantSubscriber(IPropertyProxy propertyProxy)
        {
            propertyProxy.Subscribe(new PropertyAccessedSubscriber(OnPropertyAccessed));
        }

        protected override void OnPropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            if (consumer == null || !consumer.IsAlive())
            {
                return;
            }

            base.OnPropertyAccessedEvent(sender, e);
        }

        protected async override ValueTask<bool> TryInvokeAsync(ComputedValueChangedArgs e)
        {
            if (consumer == null)
            {
                return false;
            }

            if (!consumer.IsAlive())
            {
                return true;
            }

            await consumer.ForceUpdate();
            return true;
        }

        protected override async ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedArgs e)
        {
            if (consumer == null)
            {
                return false;
            }

            if (!consumer.IsAlive())
            {
                return true;
            }

            IObservableProperty observableProperty = e.ObservableProperty;
            string propertyName = e.PropertyInfo.Name;
            if (observableContainers.TryGetValue(observableProperty, out IObservableContainer container))
            {
                if (container.IsSubscribed(propertyName))
                {
                    await consumer.ForceUpdate();
                    return true;
                }
            }

            return false;
        }

        protected override async ValueTask<bool> TryInvokeAsync(ObservableCollectionItemsChangedArgs e)
        {
            if (consumer == null)
            {
                return false;
            }

            if (!consumer.IsAlive())
            {
                return true;
            }

            if (e.ObservableCollection.ElementType == typeof(Task))
            {
                foreach (var task in e.ItemsAdded.Cast<Task>())
                {
                    _ = task.ContinueWith(t => { 
                        consumer?.ForceUpdate(); 
                    });
                }
                return true;
            }

            if (e.NewCount != e.OldCount || e.ItemsAdded.Any() || e.ItemsRemoved.Any())
            {
                await consumer.ForceUpdate();
                return true;
            }

            return false;
        }
    }
}
