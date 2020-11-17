using System;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public interface IClientScopeBuilder
    {
        IClientContractCollection Contracts { get; }

        void UseNamedHttpClient(string httpClientName);

        void UseHttpClientResolver<TResolver>() where TResolver : IHttpClientResolver;

        IContractScope Build();
    }
}