using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions.Events
{
    public class ObservableCollectionItemsChangedEventArgs
    {
        public IObservableCollection ObservableCollection { get; set; }

        public int OldCount { get; set; }

        public int NewCount { get; set; }

        public IEnumerable<object> ItemsAdded { get; set; }

        public IEnumerable<object> ItemsRemoved { get; set; }
    }
}
