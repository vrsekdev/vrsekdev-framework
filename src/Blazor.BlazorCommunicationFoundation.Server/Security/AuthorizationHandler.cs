﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security
{
    internal class AuthorizationHandler : IAuthorizationHandler
    {
        private readonly IPolicyEvaluator policyEvaluator;

        public AuthorizationHandler(IPolicyEvaluator policyEvaluator)
        {
            this.policyEvaluator = policyEvaluator;
        }

        public async Task<bool> AuthorizeAsync(HttpContext httpContext, AuthorizationContext authorizationContext)
        {
            AuthorizationPolicy policy = authorizationContext?.Policy;
            if (policy == null)
            {
                // authorization not required
                return true;
            }

            AuthenticateResult authenticationResult = await policyEvaluator.AuthenticateAsync(policy, httpContext);
            PolicyAuthorizationResult authorizationResult = await policyEvaluator.AuthorizeAsync(policy, authenticationResult, httpContext, null);
            if (authorizationResult.Challenged)
            {
                await httpContext.ChallengeAsync();
                return false;
            }
            else if (authorizationResult.Forbidden)
            {
                await httpContext.ForbidAsync();
                return false;
            }

            return true;
        }
    }
}
