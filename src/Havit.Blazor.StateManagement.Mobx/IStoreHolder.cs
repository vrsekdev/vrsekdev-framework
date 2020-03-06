using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal interface IStoreHolder<TStore>
        where TStore : class
    {
        ObservableProperty RootObservableProperty { get; }

        event EventHandler<StatePropertyChangedEventArgs> StatePropertyChangedEvent;

        event EventHandler<CollectionItemsChangedEventArgs> CollectionItemsChangedEvent;

        ObservableProperty CreateObservableProperty(Type type);
    }
}
