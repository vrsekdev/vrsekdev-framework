using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Reactables
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

        protected override ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedEventArgs e)
        {
            if (!initialized)
            {
                initialized = true;
                if (reactable.RequiresInitialInvoke())
                {
                    InvokeInternal(true);
                    return new ValueTask<bool>(true);
                }
            }

            if (observableContainers.TryGetValue(e.ObservableProperty, out IObservableContainer container))
            {
                if (container.IsSubscribed(e.PropertyInfo.Name))
                {
                    InvokeInternal(false);
                    return new ValueTask<bool>(true);
                }
            }

            return new ValueTask<bool>(false);
        }

        protected override ValueTask<bool> TryInvokeAsync(ObservableCollectionItemsChangedEventArgs e)
        {
            if (!initialized)
            {
                initialized = true;
                if (reactable.RequiresInitialInvoke())
                {
                    InvokeInternal(true);
                    return new ValueTask<bool>(true);
                }
            }

            /*if (e.NewCount != e.OldCount || e.ItemsAdded.Any() || e.ItemsRemoved.Any())
            {
                InvokeInternal(false);
                return new ValueTask<bool>(true);
            }*/

            return new ValueTask<bool>(false);
        }

        protected virtual void InvokeInternal(bool isInitialInvoke)
        {
            reactable.Invoke();
        }
    }
}
