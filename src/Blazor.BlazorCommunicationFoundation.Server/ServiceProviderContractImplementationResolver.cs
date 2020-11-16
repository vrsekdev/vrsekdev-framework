using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Infrastructure;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    internal class ServiceProviderContractImplementationResolver : IContractImplementationResolver
    {
        private readonly IContractImplementationStore contractImplementationStore;
        private readonly IServiceProvider serviceProvider;

        public ServiceProviderContractImplementationResolver(
            IContractImplementationStore contractImplementationStore,
            IServiceProvider serviceProvider)
        {
            this.contractImplementationStore = contractImplementationStore;
            this.serviceProvider = serviceProvider;
        }

        public object Resolve(Type contractType)
        {
            HashSet<Type> registeredTypes = contractImplementationStore.GetRegisteredTypes();
            if (!registeredTypes.Contains(contractType))
            {
                throw new ContractNotRegisteredException(contractType);
            }

            return serviceProvider.GetRequiredService(contractType);
        }
    }
}
