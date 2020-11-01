using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions.Events
{
    public class BatchObservableChangeArgs
    {
        public List<ComputedValueChangedArgs> ComputedValueChanges { get; set; }

        public List<ObservablePropertyStateChangedArgs> PropertyChanges { get; set; }

        public List<ObservableCollectionItemsChangedArgs> CollectionChanges { get; set; }
    }
}
