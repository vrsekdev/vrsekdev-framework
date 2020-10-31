using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public class InvocationRequestArgument
    {
        public ArgumentBindingInfo BindingInfo { get; set; }

        public byte[] Value { get; set; }
    }
}
