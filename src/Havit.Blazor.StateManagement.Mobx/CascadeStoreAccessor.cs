using Havit.Blazor.StateManagement.Mobx.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    public class CascadeStoreAccessor<TStore> : IStoreAccessor<TStore>
        where TStore : class
    {
        public TStore Store => throw CreateException();

        public T CreateObservable<T>() where T : class
        {
            throw CreateException();
        }

        public void SetConsumer(BlazorMobxComponentBase<TStore> consumer)
        {
            //throw CreateException();
        }

        public void SetConsumer(ComponentBase consumer)
        {
            //throw CreateException();
        }

        public void Dispose()
        {
            // NOOP
        }

        private Exception CreateException() => new InvalidOperationException("Store accessor is not available for cascade lifestyle");
    }
}
