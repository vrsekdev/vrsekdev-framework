using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding
{
    public static class BindingHelper
    {
        public static string CreateRequestPath(string typeIdentifier, string methodIdentifier)
        {
            return $"/{typeIdentifier}/{methodIdentifier}";
        }
    }
}
