using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions.Events
{
    public class ComputedValueChangedEventArgs
    {
        public IComputedValue ComputedValue { get; set; }
    }
}
