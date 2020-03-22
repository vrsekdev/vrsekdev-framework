using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public interface IObservableFactoryFactory
    {
        IObservableFactory Create(
            EventHandler<ObservablePropertyStateChangedEventArgs> observablePropertyStateChangedEventArgs,
            EventHandler<ObservableCollectionItemsChangedEventArgs> observableCollectionItemsChangedEventArgs);
    }
}
