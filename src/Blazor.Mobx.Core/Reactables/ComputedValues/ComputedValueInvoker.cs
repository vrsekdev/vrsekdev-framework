using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Reactables.ComputedValues
{
    internal class ComputedValueInvoker<TStore> : ReactableInvoker<TStore>
        where TStore : class
    {
        private readonly EventHandler<ComputedValueChangedEventArgs> computedValueChangedEvent;
        private readonly IComputedValueInvokable computedValue;

        public ComputedValueInvoker(
            EventHandler<ComputedValueChangedEventArgs> computedValueChangedEvent,
            IComputedValueInvokable computedValue, 
            IStoreHolder<TStore> storeHolder) : base(computedValue, storeHolder)
        {
            this.computedValueChangedEvent = computedValueChangedEvent;
            this.computedValue = computedValue;
        }

        protected async override ValueTask InvokeInternalAsync(bool isInitialInvoke)
        {
            await base.InvokeInternalAsync(isInitialInvoke);

            if (!isInitialInvoke)
            {
                computedValueChangedEvent(this, new ComputedValueChangedEventArgs
                {
                    ComputedValue = computedValue
                });
            }
        }
    }
}
