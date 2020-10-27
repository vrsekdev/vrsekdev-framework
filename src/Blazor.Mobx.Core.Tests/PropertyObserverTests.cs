using VrsekDev.Blazor.Mobx.Extensions;
using VrsekDev.Blazor.Mobx.PropertyObservers;
using VrsekDev.Blazor.Mobx.Tests.Fakes;
using VrsekDev.Blazor.Mobx.Tests.ObservableProperties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Tests
{
    [TestClass]
    public class PropertyObserverTests
    {
        private ServiceProvider serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDefaultMobxProperties();

            serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void PropertyObserver_ValueChanged_InvokeComponent()
        {
            // Arrange
            var observerFactory = serviceProvider.GetRequiredService<IPropertyObserverFactory>();
            var observer = observerFactory.Create<SimpleObservableProperty>();

            var component = new FakeBlazorComponent();
            observer.SetConsumer(component);
            var observable = observer.WrappedInstance;

            // Act
            // Access the property
            _ = observable.StringValue;
            observable.StringValue = "test";

            // Assert
            Assert.AreEqual(1, component.invokeCount);
        }

        public T CreateObservable<T>(T instance)
           where T : class
        {
            var observerFactory = serviceProvider.GetRequiredService<IPropertyObserverFactory>();

            var observer = observerFactory.Create<T>();
            observer.SetConsumer(new FakeBlazorComponent());
            observer.InitializeValues(instance);

            return observer.WrappedInstance;
        }

        [TestMethod]
        public void PropertyObserver_HasInstance_InitializeValues()
        {
            // Arrange
            var instance = new SimpleObservableProperty
            {
                StringValue = "test"
            };

            // Act
            var observable = CreateObservable(instance);

            // Assert
            Assert.AreEqual(instance.StringValue, observable.StringValue);
        }
    }
}
