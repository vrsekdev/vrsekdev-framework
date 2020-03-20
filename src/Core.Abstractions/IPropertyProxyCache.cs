using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IPropertyProxyCache<T> : IEnumerable<T>
    {

        public bool TryGetValue(T originalValue, out T value);

        T Insert(T originalValue);

        bool Remove(T boxedValue);

        internal void SubscribeAll(IPropertyAccessedSubscriber subscriber);
    }
}
