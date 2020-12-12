using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Options
{
    public interface IServerOptionsBuilder : IOptionsBuilder<ServerBCFOptions>
    {
        void AddSerializer<T>() where T : IInvocationSerializer => AddSerializer(typeof(T));

        void AddSerializer(Type type);

        IServerContractCollection Contracts { get; }

        internal IServiceCollection GetServiceCollection();
    }
}
