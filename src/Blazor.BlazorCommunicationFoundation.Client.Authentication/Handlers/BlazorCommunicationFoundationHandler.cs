using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.Infrastructure;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.Handlers
{
    public class BlazorCommunicationFoundationHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider accessTokenProvider;
        private readonly NavigationManager navigationManager;

        private AuthenticationHeaderValue cachedHeader;
        private AccessTokenRequestOptions tokenOptions;
        private AccessToken lastToken;
        private string redirectUrl;
        private Uri bcfEndpointUri;

        public BlazorCommunicationFoundationHandler(
            IAccessTokenProvider accessTokenProvider,
            NavigationManager navigationManager)
        {
            this.accessTokenProvider = accessTokenProvider;
            this.navigationManager = navigationManager;

            //bcfEndpointUri = new Uri(navigationManager.BaseUri, "/bcf/invoke");
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now;
            bool isBase = request.RequestUri.LocalPath == "/bcf/invoke";

            if (isBase)
            {
                if (lastToken == null || now >= lastToken.Expires.AddMinutes(-5))
                {
                    var tokenResult = tokenOptions != null ? 
                        await accessTokenProvider.RequestAccessToken(tokenOptions) :
                        await accessTokenProvider.RequestAccessToken();

                    if (tokenResult.TryGetToken(out var token))
                    {
                        lastToken = token;
                        redirectUrl = tokenResult.RedirectUrl;
                        cachedHeader = new AuthenticationHeaderValue("Bearer", lastToken.Value);
                    }
                }

                request.Headers.Authorization = cachedHeader;
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (isBase && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException(navigationManager, tokenOptions.ReturnUrl);
            }

            return response;
        }

        public BlazorCommunicationFoundationHandler ConfigureHandler(
            string bcfEndpointUrl = null,
            string returnUrl = null)
        {
            bcfEndpointUri = new Uri(bcfEndpointUrl ?? Path.Combine(navigationManager.BaseUri, "/bcf/invoke"));

            if (returnUrl != null)
            {
                tokenOptions = new AccessTokenRequestOptions
                {
                    ReturnUrl = returnUrl
                };
            }

            return this;
        }
    }
}
