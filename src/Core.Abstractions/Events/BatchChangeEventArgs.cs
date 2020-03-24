using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions.Events
{
    public class BatchObservableChangeEventArgs
    {
        public List<ObservablePropertyStateChangedEventArgs> PropertyChanges { get; set; }

        public List<ObservableCollectionItemsChangedEventArgs> CollectionChanges { get; set; }
    }
}
