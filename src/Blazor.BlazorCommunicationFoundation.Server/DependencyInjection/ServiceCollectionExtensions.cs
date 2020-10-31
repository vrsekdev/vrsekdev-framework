using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFServer(this IServiceCollection services)
        {
            services.AddTransient<IMethodInvoker, ReflectionMethodInvoker>();
        }
    }
}
