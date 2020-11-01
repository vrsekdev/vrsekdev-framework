using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.Mobx.Abstractions.Events;

namespace VrsekDev.Blazor.Mobx
{
    public interface IStoreSubscriber
    {
        ValueTask ComputedValueChangedAsync(ComputedValueChangedArgs args);
        ValueTask PropertyStateChangedAsync(ObservablePropertyStateChangedArgs args);
        ValueTask CollectionItemsChangedAsync(ObservableCollectionItemsChangedArgs args);
        ValueTask BatchObservableChangedAsync(BatchObservableChangeArgs args);
    }
}
