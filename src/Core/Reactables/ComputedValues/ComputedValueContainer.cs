using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables.ComputedValues
{
    public class ComputedValueContainer<TStore, TValue> : IComputedValueInvokable
    {
        private bool isInvalidated = true;
        private TValue cachedValue;
        private readonly TStore store;

        public ComputedValueContainer(
            TStore store)
        {
            this.store = store;
        }

        public bool RequiresInitialInvoke()
        {
            return true;
        }

        public void Invoke()
        {
            isInvalidated = true;
        }

        public TValue OnMethodInvoke(Func<TStore, TValue> storeFunc)
        {
            if (!isInvalidated)
                return cachedValue;

            isInvalidated = false;
            return cachedValue = storeFunc(store);
        }
    }
}
