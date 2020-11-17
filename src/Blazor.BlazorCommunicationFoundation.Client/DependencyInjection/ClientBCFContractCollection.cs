using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Emit;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    internal class ClientBCFContractCollection : IClientContractCollection
    {
        private readonly IServiceCollection services;

        public ClientBCFContractCollection(IServiceCollection services)
        {
            this.services = services;
        }

        public void AddContract<TContract>() where TContract : class
        {
            Type contractProxy = RuntimeProxyBuilder.BuildRuntimeType(typeof(TContract));

            services.AddTransient<RuntimeProxy<TContract>>();
            services.AddTransient(typeof(TContract), contractProxy);
        }
    }
}
