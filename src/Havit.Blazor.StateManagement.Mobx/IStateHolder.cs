using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    public interface IStateHolder<TState>
        where TState : class
    {
        ObservableProperty RootObservableProperty { get; }

        event EventHandler<StatePropertyChangedEventArgs> StatePropertyChangedEvent;

        event EventHandler<CollectionItemsChangedEventArgs> CollectionItemsChangedEvent;
    }
}
