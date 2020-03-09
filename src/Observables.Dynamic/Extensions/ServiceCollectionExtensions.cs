using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Observables.Dynamic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseDynamicObservables(this IServiceCollection services)
        {
            services
                .AddTransient<IPropertyObservableFactory, DynamicPropertyObservableFactory>()
                .AddTransient<IPropertyObservableWrapper, DynamicPropertyObservableWrapper>();
        }
    }
}
