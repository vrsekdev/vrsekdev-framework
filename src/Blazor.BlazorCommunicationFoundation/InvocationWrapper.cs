using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public class InvocationWrapper
    {
        public BindingInfo BindingInfo { get; set; }

        public object[] Arguments { get; set; }
    }
}
