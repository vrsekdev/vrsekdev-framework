using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFServer(this IServiceCollection services, Action<IContractCollection> contractsAction)
        {
            services.AddBlazorCommunicationFoundation();

            services.AddSingleton<IMethodInvoker, ReflectionMethodInvoker>();
            services.AddTransient<IAuthorizationContextProvider, AuthorizationContextProvider>();
            services.AddTransient<IAuthorizationHandler, AuthorizationHandler>();
            services.AddTransient<IContractImplementationResolver, ServiceProviderContractImplementationResolver>();

            BCFContractCollection contractCollection = new BCFContractCollection(services);
            contractsAction(contractCollection);
            services.AddSingleton<IContractImplementationStore, ServiceProviderContractImplementationStore>(
                serviceProvider => new ServiceProviderContractImplementationStore(contractCollection.Contracts, serviceProvider.GetRequiredService<IContractTypeSerializer>()));
        }
    }
}
