using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    internal class ServiceProviderContractImplementationStore : IContractImplementationStore
    {
        private readonly HashSet<Type> registeredTypes;

        public ServiceProviderContractImplementationStore(IEnumerable<Type> registeredTypes)
        {
            this.registeredTypes = registeredTypes.ToHashSet();
        }

        public HashSet<Type> GetRegisteredTypes()
        {
            return registeredTypes;
        }
    }
}
