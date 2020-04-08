using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx
{
    internal abstract class StoreObserverBase<TStore> : ObserverBase<TStore>
        where TStore : class
    {
        public StoreObserverBase(
            IStoreHolder<TStore> storeHolder) : base(storeHolder)
        {
            // no memory leak, because we dont store a reference to store holder
            storeHolder.ComputedValueChangedEvent += StoreHolder_ComputedValueChangedEvent;
            storeHolder.BatchObservableChangeEvent += StoreHolder_BatchObservableChangeEvent;
        }

        protected virtual async void StoreHolder_BatchObservableChangeEvent(object sender, BatchObservableChangeEventArgs e)
        {
            int computedValueChanges = e.ComputedValueChanges.Count;
            int collectionslength = e.CollectionChanges.Count;
            int propertiesLength = e.PropertyChanges.Count;

            int index = 0;
            // Invoke only once for all changes
            while ((index < computedValueChanges && !await TryInvokeAsync(e.ComputedValueChanges[index]))
                || (index < propertiesLength && !await TryInvokeAsync(e.PropertyChanges[index])) 
                || (index < collectionslength && !await TryInvokeAsync(e.CollectionChanges[index]))) index++;
        }

        private async void StoreHolder_ComputedValueChangedEvent(object sender, ComputedValueChangedEventArgs e)
        {
            await TryInvokeAsync(e);
        }

        protected abstract ValueTask<bool> TryInvokeAsync(ComputedValueChangedEventArgs e);
    }
}
