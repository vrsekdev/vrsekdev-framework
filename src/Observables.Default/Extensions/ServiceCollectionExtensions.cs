using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.ObservableProperties.Default.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseDefaultMobxObservableProperties(this IServiceCollection services)
        {
            services
                .AddSingleton<IObservableFactoryFactory, ObservableFactoryFactory>();
        }
    }
}
