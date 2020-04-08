using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.PropertyObservers
{
    internal class ObservablePropertyHolder<T> : IObservableHolder<T>
        where T : class
    {
        public event EventHandler<ObservablePropertyStateChangedEventArgs> PropertyStateChangedEvent;
        public event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        public IObservableProperty RootObservableProperty { get; }

        public ObservablePropertyHolder(
            IObservableFactoryFactory observableFactoryFactory)
        {
            var observableFactory = observableFactoryFactory.CreateFactory(
                OnPropertyStateChanged,
                OnCollectionItemsChanged);

            RootObservableProperty = observableFactory.CreateObservableProperty(typeof(T));
        }

        private void OnPropertyStateChanged(object sender, ObservablePropertyStateChangedEventArgs e)
        {
            PropertyStateChangedEvent?.Invoke(sender, e);
        }

        private void OnCollectionItemsChanged(object sender, ObservableCollectionItemsChangedEventArgs e)
        {
            CollectionItemsChangedEvent?.Invoke(sender, e);
        }
    }
}
