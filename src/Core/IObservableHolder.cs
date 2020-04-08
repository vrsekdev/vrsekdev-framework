using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx
{
    internal interface IObservableHolder<T>
    {
        event EventHandler<ObservablePropertyStateChangedEventArgs> PropertyStateChangedEvent;
        event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        IObservableProperty RootObservableProperty { get; }
    }
}
