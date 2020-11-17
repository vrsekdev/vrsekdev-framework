﻿using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options
{
    public interface IClientOptionsBuilder : IOptionsBuilder<ClientBCFOptions>
    {
        IClientContractCollection Contracts { get; }

        void UseNamedHttpClient(string httpClientName);

        void UseHttpClientResolver<TResolver>() where TResolver : IHttpClientResolver;

        void CreateScope(Action<IClientScopeBuilder> scopeBuilderAction);
    }
}
