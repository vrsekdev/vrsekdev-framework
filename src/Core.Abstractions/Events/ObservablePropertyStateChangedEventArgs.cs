using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions.Events
{
    public class ObservablePropertyStateChangedEventArgs
    {
        public PropertyInfo PropertyInfo { get; set; }

        public string PropertyName { get; set; }
    }
}
