using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure;
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

            InvocationRequest invocationRequest = await invocationSerializer.DeserializeAsync<InvocationRequest>(httpContext.Request.Body);

            Type contractType;
            try
            {
                contractType = contractBinder.BindContractType(invocationRequest.BindingInfo.BindingIdentifier);
            }
            catch (ContractNotRegisteredException)
            {
                httpContext.Response.StatusCode = 455; // Custom http status code
                return;
            }
            object contractImplementation = contractImplementationResolver.Resolve(contractType);

            MethodInfo methodInfo = contractBinder.BindContractMethod(invocationRequest.BindingInfo.BindingIdentifier);

            AuthorizationContext authorizationContext = await authorizationContextProvider.GetAuthorizationContextAsync(contractImplementation, methodInfo);
            if (!await authorizationHandler.AuthorizeAsync(httpContext, authorizationContext))
            {
                return;
            }

            var result = await methodInvoker.InvokeAsync(methodInfo, contractImplementation, invocationRequest.Arguments);
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
