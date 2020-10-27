using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables.ComputedValues
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

        protected override void InvokeInternal(bool isInitialInvoke)
        {
            base.InvokeInternal(isInitialInvoke);

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
