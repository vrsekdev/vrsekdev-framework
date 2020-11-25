using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Metadata;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Middlewares
{
    public class InvokeMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IContractBinder contractBinder;
        private readonly IMethodInvoker methodInvoker;

        public InvokeMiddleware(
            RequestDelegate next,
            IInvocationSerializer invocationSerializer,
            IContractBinder contractBinder,
            IMethodInvoker methodInvoker)
        {
            this.next = next;
            this.invocationSerializer = invocationSerializer;
            this.contractBinder = contractBinder;
            this.methodInvoker = methodInvoker;
        }

        public async Task Invoke(HttpContext httpContext, 
            IContractImplementationResolver contractImplementationResolver,
            IAuthorizationContextProvider authorizationContextProvider,
            IAuthorizationHandler authorizationHandler)
        {
            if (!httpContext.Request.Path.Value.StartsWith("/bcf/invoke"))
            {
                await next(httpContext);
                return;
            }

            string bindingIdentifier = GetBindingIdentifierFromQuery(httpContext.Request.QueryString.Value);
            if (bindingIdentifier == null)
            {
                httpContext.Response.StatusCode = CustomStatusCodes.BindingIdentifierNotFound; // Custom http status code
                return;
            }

            Type contractType;
            try
            {
                contractType = contractBinder.BindContractType(bindingIdentifier);
            }
            catch (ContractNotRegisteredException)
            {
                httpContext.Response.StatusCode = CustomStatusCodes.ContractNotRegistered; // Custom http status code
                return;
            }
            object contractImplementation = contractImplementationResolver.Resolve(contractType);

            MethodInfo methodInfo = contractBinder.BindContractMethod(bindingIdentifier);

            Dictionary<string, Type> argumentMapping = methodInfo.GetParameters().ToDictionary(x => x.Name, x => x.ParameterType);
            ArgumentDictionary arguments = await invocationSerializer.DeserializeArgumentsAsync(httpContext.Request.Body, argumentMapping);

            AuthorizationContext authorizationContext = await authorizationContextProvider.GetAuthorizationContextAsync(contractImplementation, methodInfo);
            if (!await authorizationHandler.AuthorizeAsync(httpContext, authorizationContext))
            {
                return;
            }

            var result = await methodInvoker.InvokeAsync(methodInfo, contractImplementation, arguments.Values.ToArray());
            if (result == null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }

            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            await invocationSerializer.SerializeAsync(httpContext.Response.Body, result.GetType(), result);
        }

        private static string GetBindingIdentifierFromQuery(string queryString)
        {
            return HttpUtility.ParseQueryString(queryString)[RequestMetadata.BindingIdentifier];
        }
    }
}
