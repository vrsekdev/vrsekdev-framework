using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.Dynamic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseDynamicPropertyObservables(this IServiceCollection services)
        {
            services
                .AddTransient<IPropertyObservableFactory, DynamicPropertyObservableFactory>()
                .AddTransient<IPropertyObservableWrapper, DynamicPropertyObservableWrapper>();
        }
    }
}
