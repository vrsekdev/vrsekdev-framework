using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    public class ServiceProviderContractImplementationResolver : IContractImplementationResolver
    {
        private readonly IServiceProvider serviceProvider;

        public ServiceProviderContractImplementationResolver(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object Resolve(Type contractType)
        {
            return serviceProvider.GetRequiredService(contractType);
        }
    }
}
