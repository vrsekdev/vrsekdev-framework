using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.PropertyObservers
{
    internal class PropertyObserverFactory : IPropertyObserverFactory
    {
        private readonly IObservableFactoryFactory observableFactoryFactory;
        private readonly IPropertyProxyFactory propertyProxyFactory;
        private readonly IPropertyProxyWrapper propertyProxyWrapper;

        public PropertyObserverFactory(
            IObservableFactoryFactory observableFactoryFactory,
            IPropertyProxyFactory propertyProxyFactory,
            IPropertyProxyWrapper propertyProxyWrapper)
        {
            this.observableFactoryFactory = observableFactoryFactory;
            this.propertyProxyFactory = propertyProxyFactory;
            this.propertyProxyWrapper = propertyProxyWrapper;
        }

        public PropertyObserver<T> Create<T>() where T : class
        {
            var holder = new ObservablePropertyHolder<T>(observableFactoryFactory);
            return new PropertyObserver<T>(holder, propertyProxyFactory, propertyProxyWrapper);
        }
    }
}
