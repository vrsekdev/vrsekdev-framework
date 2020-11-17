using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Emit;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    internal class ClientBCFContractCollection : IClientContractCollection
    {
        private readonly IServiceCollection services;

        public ClientBCFContractCollection(
            IServiceCollection services)
        {
            this.services = services;
        }

        public HashSet<Type> ContractTypes { get; } = new HashSet<Type>();

        public void AddContract<TContract>() where TContract : class
        {
            Type contractType = typeof(TContract);
            if (ContractTypes.Contains(contractType))
            {
                throw new ArgumentException($"Contract `{contractType.Name}` has already been registered.");
            }

            ContractTypes.Add(contractType);

            Type contractProxy = RuntimeProxyBuilder.BuildRuntimeType(contractType);

            services.AddTransient<RuntimeProxy<TContract>>();
            services.AddTransient(contractType, contractProxy);
        }
    }
}
