using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public interface IServerContractCollection : IContractCollection, IServiceCollection
    {
    }
}
