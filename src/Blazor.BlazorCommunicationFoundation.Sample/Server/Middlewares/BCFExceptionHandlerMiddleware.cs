using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Middlewares;

namespace Blazor.BlazorCommunicationFoundation.Sample.Server.Middlewares
{
    public class BCFExceptionHandlerMiddleware : ExceptionHandlerMiddlewareBase
    {
        public BCFExceptionHandlerMiddleware(RequestDelegate next) : base(next)
        {
        }

        protected override string SerializeException(Exception e)
        {
            return e.ToString();
        }
    }
}
