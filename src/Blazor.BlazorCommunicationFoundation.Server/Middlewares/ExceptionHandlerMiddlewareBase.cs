using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Middlewares
{
    public abstract class ExceptionHandlerMiddlewareBase
    {
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddlewareBase(
            RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Path.Value.StartsWith("/bcf/invoke"))
            {
                await next(httpContext);
            }
            else
            {
                try
                {
                    await next(httpContext);
                }
                catch (Exception e)
                {
                    string exceptionDetails = SerializeException(e);

                    httpContext.Response.StatusCode = 500;
                    await httpContext.Response.WriteAsync(exceptionDetails);
                }
            }
        }

        protected abstract string SerializeException(Exception e);
    }
}
