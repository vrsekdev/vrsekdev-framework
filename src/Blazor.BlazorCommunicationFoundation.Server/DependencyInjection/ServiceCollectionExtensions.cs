using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFServer(this IServiceCollection services, Action<IServerOptionsBuilder> builderAction)
        {
            ServerBCFOptionsBuilder builder = new ServerBCFOptionsBuilder(services);
            builderAction(builder);
            BCFOptions options = builder.Build();
            ServerBCFOptions serverOptions = ((IServerOptionsBuilder)builder).Build();

            services.AddBlazorCommunicationFoundation(options);

            services.AddSingleton<IMethodInvoker, ReflectionMethodInvoker>();
            services.AddTransient<IAuthorizationContextProvider, AuthorizationContextProvider>();
            services.AddTransient<IAuthorizationHandler, AuthorizationHandler>();
            services.AddTransient<IContractImplementationResolver, ServiceProviderContractImplementationResolver>();

            services.AddSingleton<IContractBinder>(provider =>
            {
                ContractBinder contractBinder = new ContractBinder(provider.GetRequiredService<IContractBindingSerializer>());
                foreach (Type contractType in serverOptions.Contracts.ContractsTypes)
                {
                    MethodInfo[] contractMethods = contractType.GetMethods();
                    foreach (MethodInfo contractMethod in contractMethods)
                    {
                        contractBinder.AddMethodBinding(contractType, contractMethod);
                    }
                }
                return contractBinder;
            });
        }
    }
}
