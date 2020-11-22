using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractImplementationAttribute : Attribute
    {
        public ServiceLifetime? Lifetime { get; set; }
    }
}
