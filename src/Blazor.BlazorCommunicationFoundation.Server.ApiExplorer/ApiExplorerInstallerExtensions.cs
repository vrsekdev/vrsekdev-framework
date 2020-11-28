using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Description;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public static class ApiExplorerInstallerExtensions
    {
        public static void AddApiExplorer(this IServerOptionsBuilder builder)
        {
            IServiceCollection services = builder.GetServiceCollection();

            ServiceDescriptor existingServiceDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IApiExplorer));
            if (existingServiceDescriptor != null)
            {
                // Ensure the original implementation type is registered
                if (!services.Any(x => x.ServiceType == existingServiceDescriptor.ImplementationType))
                {
                    // Service is not available through implementation type
                    ServiceDescriptor implementationServiceDescriptor;
                    if (existingServiceDescriptor.ImplementationFactory != null)
                    {
                        // Originally implemented via factory
                        implementationServiceDescriptor = new ServiceDescriptor(
                            existingServiceDescriptor.ImplementationType,
                            existingServiceDescriptor.ImplementationFactory,
                            existingServiceDescriptor.Lifetime);
                    }
                    else
                    {
                        // Originally implemented via constructor
                        implementationServiceDescriptor = new ServiceDescriptor(
                            existingServiceDescriptor.ImplementationType,
                            existingServiceDescriptor.ImplementationType,
                            existingServiceDescriptor.Lifetime);
                    }
                    services.Add(implementationServiceDescriptor);
                }

                // Create new decorator type with original service type as argument
                Type decoratorType = typeof(ApiExplorerDecorator<>).MakeGenericType(existingServiceDescriptor.ImplementationType);
                // Replace original service implementation with decorator
                services.Replace(new ServiceDescriptor(typeof(IApiExplorer), decoratorType, existingServiceDescriptor.Lifetime));
            }
            else
            {
                services.TryAddEnumerable(new ServiceDescriptor(typeof(IApiDescriptionProvider), typeof(ServiceApiDescriptionProvider), ServiceLifetime.Transient));
                services.AddTransient<IApiExplorer, ServiceApiExplorer>();
            }
        }
    }
}
