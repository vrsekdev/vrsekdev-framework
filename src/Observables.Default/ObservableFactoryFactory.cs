using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Observables.Default
{
    internal class ObservableFactoryFactory : IObservableFactoryFactory
    {
        public IObservableFactory CreateFactory(
            EventHandler<ObservablePropertyStateChangedEventArgs> observablePropertyStateChangedEventArgs, 
            EventHandler<ObservableCollectionItemsChangedEventArgs> observableCollectionItemsChangedEventArgs)
        {
            return new ObservableFactory(observablePropertyStateChangedEventArgs, observableCollectionItemsChangedEventArgs);
        }
    }
}
