using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public class PropertyAccessedEventArgs
    {
        public IPropertyObservable PropertyObservable { get; set; }

        public string PropertyName { get; set; }
    }
}
