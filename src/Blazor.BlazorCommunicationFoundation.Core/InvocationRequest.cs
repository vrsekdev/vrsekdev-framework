using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation
{
    public class InvocationRequest
    {
        public RequestBindingInfo BindingInfo { get; set; }

        public IDictionary<string, object> Arguments { get; set; }
    }
}
