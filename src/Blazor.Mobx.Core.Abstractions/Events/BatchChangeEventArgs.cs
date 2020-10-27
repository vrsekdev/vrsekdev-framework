using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions.Events
{
    public class BatchObservableChangeEventArgs
    {
        public List<ComputedValueChangedEventArgs> ComputedValueChanges { get; set; }

        public List<ObservablePropertyStateChangedEventArgs> PropertyChanges { get; set; }

        public List<ObservableCollectionItemsChangedEventArgs> CollectionChanges { get; set; }
    }
}
