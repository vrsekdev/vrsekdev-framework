using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public interface IServerContractCollection : IServiceCollection
    {
        HashSet<Type> ContractsTypes { get; }
    }
}
