using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions.Utils
{
    public class LazyDefault<TValue>
    {
        private bool isValueSet;
        private TValue value;
        private Lazy<TValue> lazyDefault;

        public LazyDefault(Func<TValue> lazyFunc)
        {
            lazyDefault = new Lazy<TValue>(lazyFunc);
        }

        public TValue Value
        {
            get
            {
                if (isValueSet)
                {
                    return value;
                }
                return lazyDefault.Value;
            }
            set
            {
                isValueSet = true;
                this.value = value;
            }
        }
    }
}
