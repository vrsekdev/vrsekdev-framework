using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseRuntimeTypeMobxPropertyObservables(this IServiceCollection services)
        {
            services
                .AddTransient<IPropertyObservableFactory, RuntimeTypePropertyObservableFactory>()
                .AddTransient<IPropertyObservableWrapper, RuntimeTypePropertyObservableWrapper>();
        }
    }
}
