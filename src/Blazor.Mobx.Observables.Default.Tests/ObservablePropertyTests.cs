using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Observables.Default.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VrsekDev.Blazor.Mobx.Observables.Default.Tests.Models;
using System;

namespace VrsekDev.Blazor.Mobx.Observables.Default.Tests
{
    [TestClass]
    public class ObservablePropertyTests
    {
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            services.UseDefaultMobxObservableProperties();
            serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void ObservableProperty_HasModelWithArray_OverwriteFromInstance()
        {
            // Arrange
            var instance = new ModelWithArrayProperty
            {
                StringArray = new string[0]
            };
            var observableFactoryFactory = serviceProvider.GetRequiredService<IObservableFactoryFactory>();
            var factory = observableFactoryFactory.CreateFactory(null, null);

            // Act
            var observableProperty = factory.CreateObservableProperty(typeof(ModelWithArrayProperty));
            observableProperty.OverwriteFrom(instance, false);

            // Assert
            observableProperty.TryGetMember(nameof(ModelWithArrayProperty.StringArray), out object result);
            Assert.AreEqual(instance.StringArray, result);
        }
    }
}
