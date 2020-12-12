using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public interface IClientOptionsBuilder : IOptionsBuilder<ClientBCFOptions>
    {
        IClientContractCollection Contracts { get; }

        void UseSerializer<T>() where T : IInvocationSerializer => UseSerializer(typeof(T));
        void UseSerializer(Type type);

        void UseNamedHttpClient(string httpClientName);

        void UseHttpClientResolver<TResolver>() where TResolver : IHttpClientResolver;

        void CreateScope(Action<IClientScopeBuilder> scopeBuilderAction);
    }
}
