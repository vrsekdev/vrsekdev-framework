using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBlazorCommunicationFoundation(this IServiceCollection services, BCFOptions options)
        {
            if (options.SerializerType == null)
            {
                throw new ArgumentNullException("Type is required", nameof(options.SerializerType));
            }
            if (options.ContractTypeSerializerType == null)
            {
                throw new ArgumentNullException("Type is required", nameof(options.ContractTypeSerializerType));
            }
            if (options.ContractBinderSerializerType == null)
            {
                throw new ArgumentNullException($"Type is required", nameof(options.ContractBinderSerializerType));
            }

            services.AddSingleton(typeof(IInvocationSerializer), options.SerializerType);
            services.AddSingleton(typeof(IContractTypeBindingSerializer), options.ContractTypeSerializerType);
            services.AddSingleton(typeof(IContractMethodBindingSerializer), options.ContractBinderSerializerType);
        }
    }
}
