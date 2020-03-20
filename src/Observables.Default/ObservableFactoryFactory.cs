using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Observables.Default
{
    internal class ObservableFactoryFactory : IObservableFactoryFactory
    {
        public IObservableFactory Create(
            EventHandler<ObservablePropertyStateChangedEventArgs> observablePropertyStateChangedEventArgs, 
            EventHandler<ObservableCollectionItemsChangedEventArgs> observableCollectionItemsChangedEventArgs)
        {
            return new ObservableFactory(observablePropertyStateChangedEventArgs, observableCollectionItemsChangedEventArgs);
        }
    }
}
