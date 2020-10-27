using VrsekDev.Blazor.Mobx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Observables.Default.Extensions
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
