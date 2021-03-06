﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.Infrastructure;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.Handlers
{
    public class BlazorCommunicationFoundationHandler : DelegatingHandler
    {
        private readonly IContractBinder contractBinder;
        private readonly IAccessTokenProvider accessTokenProvider;
        private readonly NavigationManager navigationManager;

        private AuthenticationHeaderValue cachedHeader;
        private AccessTokenRequestOptions tokenOptions;
        private AccessToken lastToken;

        public BlazorCommunicationFoundationHandler(
            IContractBinder contractBinder,
            IAccessTokenProvider accessTokenProvider,
            NavigationManager navigationManager)
        {
            this.contractBinder = contractBinder;
            this.accessTokenProvider = accessTokenProvider;
            this.navigationManager = navigationManager;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now;

            bool isFamiliar = contractBinder.IsPathFamiliar(request.RequestUri.LocalPath);
            if (isFamiliar)
            {
                if (lastToken == null || now >= lastToken.Expires.AddMinutes(-5))
                {
                    var tokenResult = tokenOptions != null ?
                        await accessTokenProvider.RequestAccessToken(tokenOptions) :
                        await accessTokenProvider.RequestAccessToken();

                    if (tokenResult.TryGetToken(out var token))
                    {
                        lastToken = token;
                        cachedHeader = new AuthenticationHeaderValue("Bearer", lastToken.Value);
                    }
                }

                request.Headers.Authorization = cachedHeader;
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (isFamiliar && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException(navigationManager, tokenOptions.ReturnUrl);
            }

            return response;
        }

        public BlazorCommunicationFoundationHandler ConfigureHandler(
            string loginUrl = null)
        {
            if (loginUrl != null)
            {
                tokenOptions = new AccessTokenRequestOptions
                {
                    ReturnUrl = loginUrl
                };
            }

            return this;
        }
    }
}
