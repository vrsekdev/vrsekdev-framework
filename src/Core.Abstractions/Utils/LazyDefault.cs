using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions.Utils
{
    public class LazyDefault<TValue>
    {
        private TValue value;
        private Lazy<TValue> lazyDefault;

        public LazyDefault(Func<TValue> lazyFunc)
        {
            lazyDefault = new Lazy<TValue>(lazyFunc);
        }

        public bool IsValueSet { get; private set; }

        public TValue Value
        {
            get
            {
                if (IsValueSet)
                {
                    return value;
                }
                return lazyDefault.Value;
            }
            set
            {
                IsValueSet = true;
                this.value = value;
            }
        }
    }
}
