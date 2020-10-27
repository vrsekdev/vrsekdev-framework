using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Observables.Default
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
