using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables.ComputedValues
{
    public class ComputedValueContainer<TStore, TValue> : IInvokableReactable
    {
        private bool isInitialized, isInvalidated = true;
        private TValue cachedValue;
        private readonly TStore store;

        public ComputedValueContainer(
            TStore store)
        {
            this.store = store;
        }

        public bool ShouldInvoke()
        {
            return !isInitialized;
        }

        public void Invoke()
        {
            isInvalidated = true;
        }

        public TValue OnMethodInvoke(Func<TStore, TValue> storeFunc)
        {
            if (!isInitialized)
                isInitialized = true;
            if (!isInvalidated)
                return cachedValue;

            isInvalidated = false;
            return cachedValue = storeFunc(store);
        }
    }
}
