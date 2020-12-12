using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Abstractions.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Options;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBCFServer(this IServiceCollection services, Action<IServerOptionsBuilder> builderAction)
        {
            IServerOptionsBuilder builder = new ServerBCFOptionsBuilder(services);
            builderAction(builder);
            ServerBCFOptions serverOptions = builder.Build();

            services.AddBlazorCommunicationFoundation(serverOptions);

            services.TryAddEnumerable(serverOptions.InvocationSerializerTypes.Select(type => new ServiceDescriptor(typeof(IInvocationSerializer), type, ServiceLifetime.Singleton)));
            services.AddSingleton<IMethodInvoker, DelegateMethodInvoker>();
            services.AddTransient<IAuthorizationContextProvider, AuthorizationContextProvider>();
            services.AddTransient<IAuthorizationHandler, AuthorizationHandler>();
            services.AddTransient<IContractImplementationResolver, ServiceProviderContractImplementationResolver>();
            services.AddTransient<ContractMethodInvocationHandler>();
        }
    }
}
