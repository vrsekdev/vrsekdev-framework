using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (options.InvocationSerializerTypes == null)
            {
                throw new ArgumentNullException("Type is required", nameof(options.InvocationSerializerTypes));
            }
            if (options.ContractTypeSerializerType == null)
            {
                throw new ArgumentNullException("Type is required", nameof(options.ContractTypeSerializerType));
            }
            if (options.ContractBinderSerializerType == null)
            {
                throw new ArgumentNullException($"Type is required", nameof(options.ContractBinderSerializerType));
            }

            services.TryAddEnumerable(options.InvocationSerializerTypes.Select(type => new ServiceDescriptor(typeof(IInvocationSerializer), type, ServiceLifetime.Singleton)));
            services.AddSingleton(typeof(IContractTypeBindingSerializer), options.ContractTypeSerializerType);
            services.AddSingleton(typeof(IContractMethodBindingSerializer), options.ContractBinderSerializerType);
        }
    }
}
