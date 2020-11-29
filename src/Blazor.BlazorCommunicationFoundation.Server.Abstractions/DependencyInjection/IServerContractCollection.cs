using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.DependencyInjection
{
    public interface IServerContractCollection : IContractCollection, IServiceCollection
    {
    }
}
