using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public interface IContractCollection : IServiceCollection
    {
        HashSet<Type> Contracts { get; }
    }
}
