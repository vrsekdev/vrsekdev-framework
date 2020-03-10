using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions.Events
{
    public class ObservablePropertyStateChangedEventArgs
    {
        IObservableProperty ObservableProperty { get; set; }

        public string PropertyName { get; set; }
    }
}
