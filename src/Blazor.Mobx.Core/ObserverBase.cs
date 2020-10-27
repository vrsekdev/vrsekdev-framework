﻿using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx
{
    internal abstract class ObserverBase<T>
        where T : class
    {
        private event EventHandler<PropertyAccessedEventArgs> PropertyAccessedEvent;

        protected Dictionary<IObservableProperty, IObservableContainer> observableContainers = new Dictionary<IObservableProperty, IObservableContainer>();

        public ObserverBase(
            IObservableHolder<T> observableHolder)
        {
            // no memory leak, because we dont store a reference to store holder
            observableHolder.CollectionItemsChangedEvent += StoreHolder_CollectionItemsChangedEvent;
            observableHolder.PropertyStateChangedEvent += StoreHolder_PropertyStateChangedEvent;
            observableHolder.ComputedValueChangedEvent += StoreHolder_ComputedValueChangedEvent;
            observableHolder.BatchObservableChangeEvent += StoreHolder_BatchObservableChangeEvent;
            PropertyAccessedEvent += OnPropertyAccessedEvent;
        }

        protected void OnPropertyAccessed(object sender, PropertyAccessedEventArgs e)
        {
            PropertyAccessedEvent?.Invoke(sender, e);
        }

        protected virtual void OnPropertyAccessedEvent(object sender, PropertyAccessedEventArgs e)
        {
            IObservableProperty observableProperty = e.PropertyProxy.ObservableProperty;
            if (!observableContainers.TryGetValue(observableProperty, out IObservableContainer container))
            {
                container = new ObservableContainer();
                observableContainers.Add(observableProperty, container);
            }

            container.OnPropertyAccessed(e.PropertyName);
        }

        private async void StoreHolder_PropertyStateChangedEvent(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            await TryInvokeAsync(e);
        }

        private async void StoreHolder_CollectionItemsChangedEvent(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            await TryInvokeAsync(e);
        }

        private async void StoreHolder_ComputedValueChangedEvent(object sender, ComputedValueChangedEventArgs e)
        {
            await TryInvokeAsync(e);
        }

        protected virtual async void StoreHolder_BatchObservableChangeEvent(object sender, BatchObservableChangeEventArgs e)
        {
            int computedValueChanges = e.ComputedValueChanges.Count;
            int collectionslength = e.CollectionChanges.Count;
            int propertiesLength = e.PropertyChanges.Count;

            int index = 0;
            // Invoke only once for all changes
            while ((index < computedValueChanges && !await TryInvokeAsync(e.ComputedValueChanges[index]))
                || (index < propertiesLength && !await TryInvokeAsync(e.PropertyChanges[index]))
                || (index < collectionslength && !await TryInvokeAsync(e.CollectionChanges[index]))) index++;
        }

        protected abstract ValueTask<bool> TryInvokeAsync(ComputedValueChangedEventArgs e);

        protected abstract ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedEventArgs e);

        protected abstract ValueTask<bool> TryInvokeAsync(ObservableCollectionItemsChangedEventArgs e);
    }
}