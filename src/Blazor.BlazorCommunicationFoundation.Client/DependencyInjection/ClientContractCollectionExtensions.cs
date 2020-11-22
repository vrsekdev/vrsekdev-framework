using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public static class ClientContractCollectionExtensions
    {
        private static Dictionary<Assembly, IEnumerable<Type>> contractTypesCache = new Dictionary<Assembly, IEnumerable<Type>>();

        /// <summary>
        /// Search for contracts with <see cref="ContractAttribute"/> and <paramref name="areaName"/> (optional)
        /// </summary>
        public static void AddContractsByAttribute(this IClientContractCollection contractCollection, Assembly assembly, string areaName = null)
        {
            if (!contractTypesCache.TryGetValue(assembly, out IEnumerable<Type> contractTypes))
            {
                contractTypes = assembly.GetTypes().Where(x => x.GetCustomAttribute<ContractAttribute>() != null);
                contractTypesCache.Add(assembly, contractTypes);
            }

            if (areaName != null)
            {
                contractTypes = contractTypes.Where(x => x.GetCustomAttribute<ContractAttribute>().Area == areaName);
            }

            foreach (Type contractType in contractTypes)
            {
                contractCollection.AddContract(contractType);
            }
        }
    }
}
