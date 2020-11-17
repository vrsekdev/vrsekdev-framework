using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Infrastructure;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Middlewares
{
    public class InvokeMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IInvocationRequestArgumentSerializer argumentSerializer;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IMethodBinder methodBinder;
        private readonly IMethodInvoker methodInvoker;

        public InvokeMiddleware(
            RequestDelegate next,
            IInvocationRequestArgumentSerializer argumentSerializer,
            IInvocationSerializer invocationSerializer,
            IMethodBinder methodBinder,
            IMethodInvoker methodInvoker)
        {
            this.next = next;
            this.argumentSerializer = argumentSerializer;
            this.invocationSerializer = invocationSerializer;
            this.methodBinder = methodBinder;
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

            InvocationRequest invocationRequest = await invocationSerializer.DeserializeAsync<InvocationRequest>(httpContext.Request.Body);

            object contractImplementation;
            try
            {
                contractImplementation = contractImplementationResolver.Resolve(invocationRequest.BindingInfo.TypeIdentifier);
            }
            catch (ContractNotRegisteredException)
            {
                httpContext.Response.StatusCode = 455; // Custom http status code
                return;
            }

            MethodInfo methodInfo = methodBinder.BindMethod(contractImplementation.GetType(), invocationRequest.BindingInfo.MethodName, invocationRequest.Arguments.Select(x => x.BindingInfo).ToArray());

            AuthorizationContext authorizationContext = await authorizationContextProvider.GetAuthorizationContextAsync(contractImplementation, methodInfo);
            if (!await authorizationHandler.AuthorizeAsync(httpContext, authorizationContext))
            {
                return;
            }

            object[] arguments = argumentSerializer.DeserializeArguments(methodInfo.GetParameters(), invocationRequest.Arguments);
            var result = await methodInvoker.InvokeAsync(methodInfo, contractImplementation, arguments);
            if (result == null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }

            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            await invocationSerializer.SerializeAsync(httpContext.Response.Body, result.GetType(), result);
        }
    }
}
