using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables.ComputedValues
{
    public class ComputedValueContainer<TStore, TValue> : ObservableContainer, IInvokableReactable
    {
        private bool isInitialized, isInvalidated = true;
        private TValue cachedValue;
        private readonly TStore store;

        public ComputedValueContainer(
            TStore store)
        {
            this.store = store;
        }

        public override bool IsSubscribed(string propertyName)
        {
            if (!isInitialized)
            {
                // behave promiscous when not initialized
                return true;
            }

            return base.IsSubscribed(propertyName);
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
