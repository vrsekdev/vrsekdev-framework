using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Reactables
{
    internal class ReactableInvoker<TStore> : ObserverBase<TStore>
        where TStore : class
    {
        private readonly IInvokableReactable reactable;
        private bool initialized = false;

        public ReactableInvoker(
            IInvokableReactable reactable,
            IStoreHolder<TStore> storeHolder) : base(storeHolder)
        {
            this.reactable = reactable;
        }

        public void PlantSubscriber(IPropertyProxy propertyProxy)
        {
            propertyProxy.Subscribe(new PropertyAccessedSubscriber(OnPropertyAccessed));
        }

        protected async override ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedArgs e)
        {
            if (!initialized)
            {
                initialized = true;
                if (reactable.RequiresInitialInvoke())
                {
                    await InvokeInternalAsync(true);
                    return true;
                }
            }

            if (observableContainers.TryGetValue(e.ObservableProperty, out IObservableContainer container))
            {
                if (container.IsSubscribed(e.PropertyInfo.Name))
                {
                    await InvokeInternalAsync(false);
                    return true;
                }
            }

            return false;
        }

        protected async override ValueTask<bool> TryInvokeAsync(ObservableCollectionItemsChangedArgs e)
        {
            if (!initialized)
            {
                initialized = true;
                if (reactable.RequiresInitialInvoke())
                {
                    await InvokeInternalAsync(true);
                    return true;
                }
            }

            /*if (e.NewCount != e.OldCount || e.ItemsAdded.Any() || e.ItemsRemoved.Any())
            {
                InvokeInternal(false);
                return new ValueTask<bool>(true);
            }*/

            return false;
        }

        protected async override ValueTask<bool> TryInvokeAsync(ComputedValueChangedArgs e)
        {
            if (!initialized)
            {
                initialized = true;
                if (reactable.RequiresInitialInvoke())
                {
                    await InvokeInternalAsync(true);
                    return true;
                }
            }

            await InvokeInternalAsync(false);
            return true;
        }

        protected virtual ValueTask InvokeInternalAsync(bool isInitialInvoke)
        {
            return reactable.InvokeAsync();
        }
    }
}
