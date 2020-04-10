﻿using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.StoreAccessors
{
    public class CascadeStoreAccessor<TStore> : IStoreAccessor<TStore>
        where TStore : class
    {
        public TStore Store => throw CreateException();

        public T CreateObservable<T>() where T : class
        {
            throw CreateException();
        }

        public T CreateObservable<T>(T instance) where T : class
        {
            throw CreateException();
        }

        public void SetConsumer(IBlazorMobxComponent consumer)
        {
            //throw CreateException();
        }

        public void SetConsumer(ComponentBase consumer)
        {
            //throw CreateException();
        }

        public void ResetStore()
        {
            throw CreateException();
        }

        public void Dispose()
        {
            // NOOP
        }

        private Exception CreateException() => new InvalidOperationException("Store accessor is not available for cascade lifestyle. Use cascade parameter to inject store accessor.");
    }
}
