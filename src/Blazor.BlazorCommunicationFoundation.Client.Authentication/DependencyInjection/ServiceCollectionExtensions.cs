using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.Handlers;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBFCAuthentication(this IServiceCollection services)
        {
            services.AddTransient<BlazorCommunicationFoundationHandler>();
        }
    }
}
