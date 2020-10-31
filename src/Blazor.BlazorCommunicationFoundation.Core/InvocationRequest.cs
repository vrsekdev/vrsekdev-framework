using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public class InvocationRequest
    {
        public RequestBindingInfo BindingInfo { get; set; }

        public InvocationRequestArgument[] Arguments { get; set; }
    }
}
