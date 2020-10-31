using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBlazorCommunicationFoundation(this IServiceCollection services)
        {
            services.AddSingleton<IInvocationSerializer, MessagePackInvocationSerializer>();
        }
    }
}
