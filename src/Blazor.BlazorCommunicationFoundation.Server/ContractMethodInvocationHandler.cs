using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    public class ContractMethodInvocationHandler
    {
        private readonly IContractImplementationResolver contractImplementationResolver;
        private readonly IAuthorizationContextProvider authorizationContextProvider;
        private readonly IAuthorizationHandler authorizationHandler;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IMethodInvoker methodInvoker;

        public ContractMethodInvocationHandler(
            IContractImplementationResolver contractImplementationResolver,
            IAuthorizationContextProvider authorizationContextProvider,
            IAuthorizationHandler authorizationHandler,
            IInvocationSerializer invocationSerializer,
            IMethodInvoker methodInvoker)
        {
            this.contractImplementationResolver = contractImplementationResolver;
            this.authorizationContextProvider = authorizationContextProvider;
            this.authorizationHandler = authorizationHandler;
            this.invocationSerializer = invocationSerializer;
            this.methodInvoker = methodInvoker;
        }

        public async Task Invoke(HttpContext httpContext, ContractMethodBinding binding)
        {
            object contractImplementation = contractImplementationResolver.Resolve(binding.ContractType);

            Dictionary<string, Type> argumentMapping = binding.ContractMethodInfo.GetParameters().ToDictionary(x => x.Name, x => x.ParameterType);

            ArgumentDictionary arguments;
            if (httpContext.Request.ContentLength != 0)
            {
                arguments = await invocationSerializer.DeserializeArgumentsAsync(httpContext.Request.Body, argumentMapping);
            }
            else
            {
                arguments = new ArgumentDictionary();
            }

            AuthorizationContext authorizationContext = await authorizationContextProvider.GetAuthorizationContextAsync(contractImplementation, binding.ContractMethodInfo);
            if (!await authorizationHandler.AuthorizeAsync(httpContext, authorizationContext))
            {
                return;
            }

            var result = await methodInvoker.InvokeAsync(binding.ContractMethodInfo, contractImplementation, arguments.Values.ToArray());
            if (result == null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }

            httpContext.Response.ContentType = invocationSerializer.MediaType;
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            await invocationSerializer.SerializeAsync(httpContext.Response.Body, result.GetType(), result);
        }
    }
}
