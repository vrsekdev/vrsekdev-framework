using System;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Reactables.ComputedValues
{
    public class ComputedValueContainer<TStore, TValue> : IComputedValueInvokable
    {
        private readonly TStore store;
        private readonly bool isAsyncResultType;

        private bool isInvalidated = true;
        private TValue cachedValue;

        public ComputedValueContainer(
            TStore store)
        {
            this.store = store;
            isAsyncResultType = typeof(Abstractions.IAsyncResult).IsAssignableFrom(typeof(TValue));
        }

        public bool RequiresInitialInvoke()
        {
            return true;
        }

        public ValueTask InvokeAsync()
        {
            isInvalidated = true;
            return default;
        }

        public TValue OnMethodInvoke(Func<TStore, TValue> storeFunc)
        {
            if (!isInvalidated)
                return cachedValue;

            isInvalidated = false;
            cachedValue = storeFunc(store);
            if (isAsyncResultType)
            {
                ((Abstractions.IAsyncResult)cachedValue).UnderLyingTask.ContinueWith(x => isInvalidated = true);
            }

            return cachedValue;
        }
    }
}
