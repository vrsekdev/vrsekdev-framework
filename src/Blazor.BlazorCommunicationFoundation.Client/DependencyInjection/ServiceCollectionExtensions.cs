using ImpromptuInterface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFContract<TInterface>(this IServiceCollection services)
            where TInterface : class
        {
            services.AddTransient<DynamicProxy<TInterface>>();
            services.AddTransient(typeof(TInterface), sp => sp.GetRequiredService<DynamicProxy<TInterface>>().ActLike<TInterface>());
        }
    }
}
