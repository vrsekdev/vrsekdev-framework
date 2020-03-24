using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions.Events
{
    public class ObservablePropertyStateChangedEventArgs
    {
        public IObservableProperty ObservableProperty { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
    }
}
