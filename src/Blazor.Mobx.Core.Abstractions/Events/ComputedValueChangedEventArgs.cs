using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions.Events
{
    public class ComputedValueChangedEventArgs
    {
        public IComputedValue ComputedValue { get; set; }
    }
}
