using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    public class ContractMethodInvocationHandler
    {
        private readonly IContractImplementationResolver contractImplementationResolver;
        private readonly IAuthorizationContextProvider authorizationContextProvider;
        private readonly IAuthorizationHandler authorizationHandler;
        private readonly IMethodInvoker methodInvoker;

        private readonly Dictionary<string, IInvocationSerializer> invocationSerializers;

        public ContractMethodInvocationHandler(
            IContractImplementationResolver contractImplementationResolver,
            IAuthorizationContextProvider authorizationContextProvider,
            IAuthorizationHandler authorizationHandler,
            IEnumerable<IInvocationSerializer> invocationSerializers,
            IMethodInvoker methodInvoker)
        {
            this.contractImplementationResolver = contractImplementationResolver;
            this.authorizationContextProvider = authorizationContextProvider;
            this.authorizationHandler = authorizationHandler;
            this.methodInvoker = methodInvoker;

            this.invocationSerializers = invocationSerializers.ToDictionary(x => x.MediaType);
        }

        public async Task Invoke(HttpContext httpContext, ContractMethodBinding binding)
        {
            if (httpContext.Request.ContentType == null && httpContext.Request.ContentLength != 0)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                return;
            }

            IInvocationSerializer requestSerializer = null;
            if (httpContext.Request.ContentType != null 
                && !invocationSerializers.TryGetValue(httpContext.Request.ContentType, out requestSerializer))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                return;
            }

            IInvocationSerializer responseSerializer = null;
            StringValues acceptHeader = httpContext.Request.Headers["Accept"];
            if (acceptHeader.Count == 0 || (acceptHeader.Count == 1 && acceptHeader[0] == "*/*"))
            {
                // Use request serializer or use the first one resolved
                responseSerializer = requestSerializer ?? invocationSerializers.Values.First();
            }
            else if (!acceptHeader.Any(value => invocationSerializers.TryGetValue(value, out responseSerializer)))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return;
            }

            ArgumentDictionary arguments;
            Dictionary<string, Type> argumentMapping = binding.ContractMethodInfo.GetParameters().ToDictionary(x => x.Name, x => x.ParameterType);
            if (httpContext.Request.ContentLength != 0)
            {
                arguments = await requestSerializer.DeserializeArgumentsAsync(httpContext.Request.Body, argumentMapping);
            }
            else
            {
                arguments = new ArgumentDictionary();
            }

            object contractImplementation = contractImplementationResolver.Resolve(binding.ContractType);
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
            else if (responseSerializer != null)
            {
                httpContext.Response.ContentType = responseSerializer.MediaType;
                httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                await responseSerializer.SerializeAsync(httpContext.Response.Body, result.GetType(), result);
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return;
            }
        }
    }
}
