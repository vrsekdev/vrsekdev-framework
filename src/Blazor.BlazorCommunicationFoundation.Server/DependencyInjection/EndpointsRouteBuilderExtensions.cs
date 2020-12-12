using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public static class EndpointsRouteBuilderExtensions
    {
        public static void MapBlazorCommunicationFoundation(this IEndpointRouteBuilder endpoints)
        {
            IServiceProvider serviceProvider = endpoints.ServiceProvider;
            IContractBinder contractBinder = serviceProvider.GetRequiredService<IContractBinder>();

            foreach (var binding in contractBinder.GetBindings())
            {
                endpoints.MapPost(binding.Key, httpContext =>
                {
                    var invocationHandler = serviceProvider.GetRequiredService<ContractMethodInvocationHandler>();

                    return invocationHandler.Invoke(httpContext, binding.Value);
                });
            }
        }
    }
}
