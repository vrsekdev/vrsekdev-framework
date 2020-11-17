using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Middlewares
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseBlazorCommunicationFoundation(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<InvokeMiddleware>();
        }
    }
}
