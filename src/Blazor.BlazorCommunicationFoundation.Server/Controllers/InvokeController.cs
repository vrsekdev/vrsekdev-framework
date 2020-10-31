using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Controllers
{
    [ApiController]
    [Route("bcf")]
    public class InvokeController : ControllerBase
    {
        private readonly IContractImplementationResolver contractImplementationResolver;
        private readonly IInvocationRequestArgumentSerializer argumentSerializer;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IAuthorizationContextProvider authorizationContextProvider;
        private readonly IAuthorizationHandler authorizationHandler;
        private readonly IMethodBinder methodBinder;
        private readonly IMethodInvoker methodInvoker;

        public InvokeController(
            IContractImplementationResolver contractImplementationResolver,
            IInvocationRequestArgumentSerializer argumentSerializer,
            IInvocationSerializer invocationSerializer,
            IAuthorizationContextProvider authorizationContextProvider,
            IAuthorizationHandler authorizationHandler,
            IMethodBinder methodBinder,
            IMethodInvoker methodInvoker)
        {
            this.contractImplementationResolver = contractImplementationResolver;
            this.argumentSerializer = argumentSerializer;
            this.invocationSerializer = invocationSerializer;
            this.authorizationContextProvider = authorizationContextProvider;
            this.authorizationHandler = authorizationHandler;
            this.methodBinder = methodBinder;
            this.methodInvoker = methodInvoker;
        }

        [HttpPost("invoke")]
        public async Task InvokeAsync()
        {
            InvocationRequest invocationRequest = await invocationSerializer.DeserializeAsync<InvocationRequest>(Request.Body);
            MethodInfo methodInfo = methodBinder.BindMethod(invocationRequest.BindingInfo, invocationRequest.Arguments.Select(x => x.BindingInfo).ToArray());
            object contractImplementation = contractImplementationResolver.Resolve(methodInfo.DeclaringType);

            AuthorizationContext authorizationContext = await authorizationContextProvider.GetAuthorizationContextAsync(contractImplementation, methodInfo);
            if (!await authorizationHandler.AuthorizeAsync(HttpContext, authorizationContext))
            {
                return;
            }

            object[] arguments = argumentSerializer.DeserializeArguments(methodInfo.GetParameters(), invocationRequest.Arguments);
            var result = await methodInvoker.InvokeAsync(methodInfo, contractImplementation, arguments);
            if (result == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            await invocationSerializer.SerializeAsync(Response.Body, result.GetType(), result);
        }
    }
}
