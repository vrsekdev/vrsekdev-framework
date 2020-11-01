using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx
{
    internal abstract class ObserverBase<T> : IStoreSubscriber
        where T : class
    {
        private event EventHandler<PropertyAccessedEventArgs> PropertyAccessedEvent;

        protected Dictionary<IObservableProperty, IObservableContainer> observableContainers = new Dictionary<IObservableProperty, IObservableContainer>();

        public ObserverBase(
            IObservableHolder<T> observableHolder)
        {
            observableHolder.Subscribers.Add(this);

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

        public async ValueTask ComputedValueChangedAsync(ComputedValueChangedArgs args)
        {
            await TryInvokeAsync(args);
        }

        public async ValueTask PropertyStateChangedAsync(ObservablePropertyStateChangedArgs args)
        {
            await TryInvokeAsync(args);
        }

        public async ValueTask CollectionItemsChangedAsync(ObservableCollectionItemsChangedArgs args)
        {
            await TryInvokeAsync(args);
        }

        public virtual async ValueTask BatchObservableChangedAsync(BatchObservableChangeArgs args)
        {
            int computedValueChanges = args.ComputedValueChanges.Count;
            int collectionslength = args.CollectionChanges.Count;
            int propertiesLength = args.PropertyChanges.Count;

            int index = 0;
            // Invoke only once for all changes
            while ((index < computedValueChanges && !await TryInvokeAsync(args.ComputedValueChanges[index]))
                || (index < propertiesLength && !await TryInvokeAsync(args.PropertyChanges[index]))
                || (index < collectionslength && !await TryInvokeAsync(args.CollectionChanges[index]))) index++;
        }

        protected abstract ValueTask<bool> TryInvokeAsync(ComputedValueChangedArgs e);

        protected abstract ValueTask<bool> TryInvokeAsync(ObservablePropertyStateChangedArgs e);

        protected abstract ValueTask<bool> TryInvokeAsync(ObservableCollectionItemsChangedArgs e);
    }
}
