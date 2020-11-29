using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public static class ApiExplorerInstallerExtensions
    {
        public static void AddApiExplorer(this IServerOptionsBuilder builder)
        {
            IServiceCollection services = builder.GetServiceCollection();

            services.TryAddEnumerable(new ServiceDescriptor(typeof(IApiDescriptionProvider), typeof(ServiceApiDescriptionProvider), ServiceLifetime.Transient));
        }
    }
}
