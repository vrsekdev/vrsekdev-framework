using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation
{
    public class InvocationRequest
    {
        public RequestBindingInfo BindingInfo { get; set; }

        public object[] Arguments { get; set; }
    }
}
