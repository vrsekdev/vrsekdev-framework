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
        private readonly HttpContext httpContext;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IMethodInvoker methodInvoker;

        public InvokeController(
            //HttpContext httpContext,
            IInvocationSerializer invocationSerializer,
            IMethodInvoker methodInvoker)
        {
            this.httpContext = httpContext;
            this.invocationSerializer = invocationSerializer;
            this.methodInvoker = methodInvoker;
        }

        [HttpPost("invoke")]
        public async Task InvokeAsync()
        {
            InvocationWrapper invocationWrapper = await invocationSerializer.DeserializeAsync<InvocationWrapper>(Request.Body);
            var result = await methodInvoker.InvokeAsync(invocationWrapper);
            if (result == null)
            {
                httpContext.Response.StatusCode = 204;
            }

            await invocationSerializer.SerializeAsync(Response.Body, result.GetType(), result);
            httpContext.Response.StatusCode = 200;
        }
    }
}
