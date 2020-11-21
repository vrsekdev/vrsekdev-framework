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
        /// <summary>
        /// Search for contracts with <see cref="Infrastructure.ContractAttribute"/> and <paramref name="areaName"/> (optional)
        /// </summary>
        public static void AddContractsByAttribute(this IClientContractCollection contractCollection, Assembly assembly, string areaName = null)
        {
            ILookup<bool, Type> types = assembly.GetTypes().ToLookup(x => x.GetCustomAttribute<ContractAttribute>() != null, x => x);

            IEnumerable<Type> contractTypes = types[true];
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
