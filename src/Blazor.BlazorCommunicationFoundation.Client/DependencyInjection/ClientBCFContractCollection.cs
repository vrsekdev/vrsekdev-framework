using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection;
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

        internal HashSet<Type> ContractTypes { get; } = new HashSet<Type>();

        IEnumerable<Type> IContractCollection.ContractTypes => ContractTypes;

        public void AddContract<TContract>() where TContract : class
        {
            Type contractType = typeof(TContract);
            if (!ContractTypes.Add(contractType))
            {
                throw new ArgumentException($"Contract `{contractType.Name}` has already been registered.");
            }

            Type contractProxy = RuntimeProxyBuilder.BuildRuntimeType(contractType);

            services.AddTransient<RuntimeProxy<TContract>>();
            services.AddTransient(contractType, contractProxy);
        }

        public void AddContract(Type contractType)
        {
            if (!ContractTypes.Add(contractType))
            {
                throw new ArgumentException($"Contract `{contractType.Name}` has already been registered.");
            }

            Type contractProxy = RuntimeProxyBuilder.BuildRuntimeType(contractType);

            Type runtimeProxyType = typeof(RuntimeProxy<>).MakeGenericType(contractType);
            services.AddTransient(runtimeProxyType);
            services.AddTransient(contractType, contractProxy);
        }
    }
}
