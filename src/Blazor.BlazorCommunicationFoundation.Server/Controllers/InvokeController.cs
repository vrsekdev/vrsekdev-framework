using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Controllers
{
    [ApiController]
    [Route("bcf")]
    public class InvokeController : ControllerBase
    {
        private readonly IInvocationRequestArgumentSerializer argumentSerializer;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IMethodBinder methodBinder;
        private readonly IMethodInvoker methodInvoker;

        public InvokeController(
            IInvocationRequestArgumentSerializer argumentSerializer,
            IInvocationSerializer invocationSerializer,
            IMethodBinder methodBinder,
            IMethodInvoker methodInvoker)
        {
            this.argumentSerializer = argumentSerializer;
            this.invocationSerializer = invocationSerializer;
            this.methodBinder = methodBinder;
            this.methodInvoker = methodInvoker;
        }

        [HttpPost("invoke")]
        public async Task InvokeAsync()
        {
            InvocationRequest invocationRequest = await invocationSerializer.DeserializeAsync<InvocationRequest>(Request.Body);
            MethodInfo methodInfo = methodBinder.BindMethod(invocationRequest.BindingInfo, invocationRequest.Arguments.Select(x => x.BindingInfo).ToArray());
            object[] arguments = argumentSerializer.DeserializeArguments(methodInfo.GetParameters(), invocationRequest.Arguments);

            var result = await methodInvoker.InvokeAsync(methodInfo, arguments);
            if (result == null)
            {
                Response.StatusCode = 204;
                return;
            }

            Response.StatusCode = 200;
            await invocationSerializer.SerializeAsync(Response.Body, result.GetType(), result);
        }
    }
}
