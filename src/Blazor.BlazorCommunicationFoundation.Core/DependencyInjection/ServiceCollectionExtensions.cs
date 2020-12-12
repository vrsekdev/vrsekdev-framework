using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBlazorCommunicationFoundation(this IServiceCollection services, BCFOptions options)
        {
            if (options.TypeBindingSerializerType == null)
            {
                throw new ArgumentNullException("Type is required", nameof(options.TypeBindingSerializerType));
            }
            if (options.MethodBindingSerializerType == null)
            {
                throw new ArgumentNullException($"Type is required", nameof(options.MethodBindingSerializerType));
            }

            services.AddSingleton(typeof(IContractTypeBindingSerializer), options.TypeBindingSerializerType);
            services.AddSingleton(typeof(IContractMethodBindingSerializer), options.MethodBindingSerializerType);

            services.AddSingleton<IContractBinder>(provider =>
            {
                ContractBinder contractBinder = new ContractBinder(
                    provider.GetRequiredService<IContractTypeBindingSerializer>(),
                    provider.GetRequiredService<IContractMethodBindingSerializer>());
                foreach (Type contractType in options.ContractTypes)
                {
                    contractBinder.AddContractBinding(contractType);
                }
                return contractBinder;
            });
        }
    }
}
