using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Proxies.RuntimeProxy
{
    internal class RuntimeProxyFactory : IPropertyProxyFactory
    {
        #region static
        private static Func<RuntimeProxyFactory, IObservableProperty, IPropertyProxy> createObservableDelegate;

        static RuntimeProxyFactory()
        {
            MethodInfo genericMethod = typeof(RuntimeProxyFactory)
                .GetMethod(nameof(CreateGeneric), BindingFlags.Static | BindingFlags.NonPublic);

            createObservableDelegate =
                (RuntimeProxyFactory instance, IObservableProperty observableProperty) => (IPropertyProxy)genericMethod
                    .MakeGenericMethod(observableProperty.ObservedType)
                    .Invoke(instance, new object[] { observableProperty });
        }

        private static IPropertyProxy CreateGeneric<T>(IObservableProperty observableProperty)
            where T : class
        {
            return new RuntimeProxyManager<T>(observableProperty);
        }
        #endregion static

        public IPropertyProxy Create(IObservableProperty observableProperty)
        {
            return createObservableDelegate(this, observableProperty);
        }
    }
}
