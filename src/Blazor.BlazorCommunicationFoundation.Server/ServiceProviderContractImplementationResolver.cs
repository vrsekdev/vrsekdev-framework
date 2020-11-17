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

        public object Resolve(string contractIdentifier)
        {
            Type registeredContract = contractImplementationStore.GetContractType(contractIdentifier);
            if (registeredContract == null)
            {
                throw new ContractNotRegisteredException(contractIdentifier);
            }

            return serviceProvider.GetRequiredService(registeredContract);
        }
    }
}
