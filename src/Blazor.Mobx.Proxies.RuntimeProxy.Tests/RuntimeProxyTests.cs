using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Extensions;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests
{
    [TestClass]
    public class RuntimeProxyTests
    {
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            services.UseMobxRuntimeProxy();
            serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void RuntimeProxy__HasModelWithArray_ReturnCorrectValue()
        {
            // Arrange
            string propName = nameof(ModelWithArrayProperty.StringArray);
            object arr = new string[0];

            var observableMock = new Mock<IObservableProperty>(MockBehavior.Strict);
            observableMock.Setup(x => x.TryGetMember(propName, out arr)).Returns(true);
            observableMock.Setup(x => x.CreateFactory()).Returns((IObservableFactory)null);
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.TrySetDefaultValue(propName, null)).Returns(true);

            var manager = new RuntimeProxyManager<ModelWithArrayProperty>(observableMock.Object);

            // Act
            var instance = manager.Implementation;

            // Assert
            Assert.AreEqual(arr, instance.StringArray);
        }
    }
}
