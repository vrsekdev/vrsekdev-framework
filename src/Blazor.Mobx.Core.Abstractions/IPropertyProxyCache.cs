using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions
{
    public interface IPropertyProxyCache<T> : IEnumerable<T>
    {

        bool TryGetValue(T originalValue, out T value);

        T Insert(T originalValue);

        bool Remove(T boxedValue);

        void Clear();

        internal void SubscribeAll(IPropertyAccessedSubscriber subscriber);

        void Recycle(IEnumerable newValues);
    }
}
