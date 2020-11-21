using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ContractAttribute : Attribute
    {
        public string Area { get; set; }
    }
}
