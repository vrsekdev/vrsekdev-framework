using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal interface IStoreHolder<TStore>
        where TStore : class
    {
        IObservableProperty RootObservableProperty { get; }

        event EventHandler<ObservablePropertyStateChangedEventArgs> StatePropertyChangedEvent;

        event EventHandler<ObservableCollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        IObservableProperty CreateObservableProperty(Type type);
    }
}
