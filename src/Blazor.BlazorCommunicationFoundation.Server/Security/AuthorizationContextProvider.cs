using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security
{
    internal class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly IAuthorizationPolicyProvider policyProvider;

        public AuthorizationContextProvider(IAuthorizationPolicyProvider policyProvider)
        {
            this.policyProvider = policyProvider;
        }

        public async Task<AuthorizationContext> GetAuthorizationContextAsync(object implementation, MethodInfo methodInfo)
        {
            Type[] parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            if (parameterTypes.Length == 0)
            {
                parameterTypes = Type.EmptyTypes;
            }
            MethodInfo concreteMethod = implementation.GetType().GetMethod(methodInfo.Name, parameterTypes);

            AuthorizeAttribute authorizeAttribute = concreteMethod.DeclaringType.GetCustomAttribute<AuthorizeAttribute>();
            // overwrite by action attribute
            authorizeAttribute = concreteMethod.GetCustomAttribute<AuthorizeAttribute>() ?? authorizeAttribute;

            if (authorizeAttribute == null)
            {
                return null;
            }

            if (authorizeAttribute.Roles != null && authorizeAttribute.Policy != null)
            {
                throw new NotSupportedException("You must specify either Roles or Policy");
            }

            AuthorizationPolicy policy;
            if (authorizeAttribute.Roles != null)
            {
                var builder = new AuthorizationPolicyBuilder(authorizeAttribute.AuthenticationSchemes.Split(','));
                builder.RequireRole(authorizeAttribute.Roles.Split(','));
                policy = builder.Build();
            }
            else if (authorizeAttribute.Policy != null)
            {
                policy = await policyProvider.GetPolicyAsync(authorizeAttribute.Policy)
                    ?? throw new ArgumentException($"Policy {authorizeAttribute.Policy} does not exist");
            }
            else
            {
                policy = await policyProvider.GetDefaultPolicyAsync();
            }

            return new AuthorizationContext
            {
                Policy = policy
            };
        }
    }
}
