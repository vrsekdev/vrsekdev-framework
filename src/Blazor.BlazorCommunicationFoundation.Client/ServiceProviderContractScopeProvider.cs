using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class ServiceProviderContractScopeProvider : IContractScopeProvider
    {
        private Dictionary<Type, IContractScope> scopes = new Dictionary<Type, IContractScope>();

        public void AddScope(Type contractType, IContractScope scope)
        {
            if (scopes.ContainsKey(contractType))
            {
                throw new ArgumentException($"Could not register scope for contract `{contractType.Name}`, because it is already registered.");
            }

            scopes.Add(contractType, scope);
        }

        public IContractScope GetScope(Type contractType)
        {
            if (!scopes.TryGetValue(contractType, out IContractScope scope))
            {
                throw new ArgumentException($"Scope for contract `{contractType.Name}` was not registered.");
            }

            return scope;
        }
    }
}
