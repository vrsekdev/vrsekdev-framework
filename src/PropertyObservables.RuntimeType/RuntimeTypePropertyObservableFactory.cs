using Havit.Blazor.StateManagement.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType
{
    internal class RuntimeTypePropertyObservableFactory : IPropertyObservableFactory
    {
        #region static
        private static Func<RuntimeTypePropertyObservableFactory, IObservableProperty, IPropertyObservable> createObservableDelegate;

        static RuntimeTypePropertyObservableFactory()
        {
            MethodInfo genericMethod = typeof(RuntimeTypePropertyObservableFactory)
                .GetMethod(nameof(CreateGeneric), BindingFlags.Static | BindingFlags.NonPublic);

            createObservableDelegate =
                (RuntimeTypePropertyObservableFactory instance, IObservableProperty observableProperty) => (IPropertyObservable)genericMethod
                    .MakeGenericMethod(observableProperty.ObservedType)
                    .Invoke(instance, new object[] { observableProperty });
        }

        private static IPropertyObservable CreateGeneric<T>(IObservableProperty observableProperty)
            where T : class
        {
            return new RuntimeTypePropertyObservableManager<T>(observableProperty);
        }
        #endregion static

        public IPropertyObservable Create(IObservableProperty observableProperty)
        {
            return createObservableDelegate(this, observableProperty);
        }
    }
}
