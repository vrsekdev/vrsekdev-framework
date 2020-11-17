using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    internal class ServiceProviderContractImplementationStore : IContractImplementationStore
    {
        private readonly Dictionary<string, Type> registeredTypes;

        public ServiceProviderContractImplementationStore(
            IEnumerable<Type> registeredTypes,
            IContractTypeSerializer contractTypeSerializer)
        {
            this.registeredTypes = registeredTypes.ToDictionary(x => contractTypeSerializer.GenerateIdentifier(x));
        }

        public Type GetContractType(string contractIdentifier)
        {
            if (!registeredTypes.TryGetValue(contractIdentifier, out Type contractType))
            {
                return null;
            }

            return contractType;
        }
    }
}
