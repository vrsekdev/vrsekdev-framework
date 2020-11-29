using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Infrastructure;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public static class ServerContractCollectionExtensions
    {
        private static Dictionary<Assembly, IEnumerable<Type>> contractImplementationsCache = new Dictionary<Assembly, IEnumerable<Type>>();

        /// <summary>
        /// Search for classes with <see cref="ContractImplementationAttribute"/>
        /// that implement contracts with <see cref="ContractAttribute"/> with <paramref name="areaName"/> (optional)
        /// </summary>
        public static void AddContractsByAttribute(this IServerContractCollection contractCollection, Assembly assembly, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, string areaName = null)
        {
            if (!contractImplementationsCache.TryGetValue(assembly, out IEnumerable<Type> implementationTypes))
            {
                implementationTypes = assembly.GetTypes().Where(x => x.GetCustomAttribute<ContractImplementationAttribute>() != null);
                contractImplementationsCache.Add(assembly, implementationTypes);
            }

            foreach (Type implementationType in implementationTypes)
            {
                ServiceLifetime implementationTypeLifetime = implementationType.GetCustomAttribute<ContractImplementationAttribute>().Lifetime ?? serviceLifetime;
                IEnumerable<Type> contractTypes = implementationType.GetInterfaces().Where(x => x.GetCustomAttribute<ContractAttribute>() != null);
                if (!contractTypes.Any())
                {
                    throw new Exception($@"Type {implementationType.FullName} has attribute {typeof(ContractImplementationAttribute).Name}, 
                                            but it does not implement any interface with attribute {typeof(ContractAttribute).Name}.");
                }

                if (areaName != null)
                {
                    contractTypes = contractTypes.Where(x => x.GetCustomAttribute<ContractAttribute>().Area == areaName);
                }

                foreach (Type contractType in contractTypes)
                {
                    contractCollection.Add(new ServiceDescriptor(contractType, implementationType, implementationTypeLifetime));
                }
            }
        }
    }
}
