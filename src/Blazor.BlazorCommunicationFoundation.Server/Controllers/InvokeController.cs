using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        private readonly IMethodInvoker methodInvoker;

        public InvokeController(
            IInvocationRequestArgumentSerializer argumentSerializer,
            IInvocationSerializer invocationSerializer,
            IMethodInvoker methodInvoker)
        {
            this.argumentSerializer = argumentSerializer;
            this.invocationSerializer = invocationSerializer;
            this.methodInvoker = methodInvoker;
        }

        [HttpPost("invoke")]
        public async Task InvokeAsync()
        {
            InvocationRequest invocationRequest = await invocationSerializer.DeserializeAsync<InvocationRequest>(Request.Body);
            object[] arguments = argumentSerializer.DeserializeArguments(invocationRequest.Arguments);
            var result = await methodInvoker.InvokeAsync(invocationRequest.BindingInfo, arguments);
            if (result == null)
            {
                Response.StatusCode = 204;
                return;
            }

            await invocationSerializer.SerializeAsync(Response.Body, result.GetType(), result);
            Response.StatusCode = 200;
        }
    }
}
