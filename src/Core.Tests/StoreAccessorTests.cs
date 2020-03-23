using Havit.Blazor.Mobx.Extensions;
using Havit.Blazor.Mobx.Tests.Fakes;
using Havit.Blazor.Mobx.Tests.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Tests
{
    [TestClass]
    public class StoreAccessorTests
    {
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            services.UseDefaultMobxProperties();
            services.AddMobxStore<ClassStoreWithDefaultValue>().LifestyleTransient();
            services.AddMobxStore<ClassStoreWithComputed>().LifestyleTransient();

            serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void Store_PropertyHasDefaultValue_PreserveDefaultValue()
        {
            // Arrange
            string defaultValue = new ClassStoreWithDefaultValue().PropertyWithDefaultValue;
            IStoreAccessor<ClassStoreWithDefaultValue> storeAccessor = serviceProvider.GetRequiredService<IStoreAccessor<ClassStoreWithDefaultValue>>();
            var consumer = new FakeBlazorComponent();
            storeAccessor.SetConsumer(consumer);

            // Assert
            Assert.AreEqual(defaultValue, storeAccessor.Store.PropertyWithDefaultValue);
        }

        [TestMethod]
        public void Store_ComputedValue_BehavePromiscous()
        {
            // Arrange
            IStoreAccessor<ClassStoreWithComputed> storeAccessor = serviceProvider.GetRequiredService<IStoreAccessor<ClassStoreWithComputed>>();
            var consumer = new FakeBlazorComponent();
            storeAccessor.SetConsumer(consumer);
            var store = storeAccessor.Store;
            int invokeCount = 0;

            // Act
            Assert.AreEqual(invokeCount, ClassStoreWithComputed.InvokeCount);
            store.ComputedMethodIncerceptingValue(); invokeCount++;

            // Assert
            Assert.AreEqual(invokeCount, ClassStoreWithComputed.InvokeCount);
        }

        [TestMethod]
        public void Store_ComputedValue_CacheValue()
        {
            // Arrange
            IStoreAccessor<ClassStoreWithComputed> storeAccessor = serviceProvider.GetRequiredService<IStoreAccessor<ClassStoreWithComputed>>();
            var consumer = new FakeBlazorComponent();
            storeAccessor.SetConsumer(consumer);
            var store = storeAccessor.Store;
            int invokeCount = 0;

            // Act
            Assert.AreEqual(invokeCount, ClassStoreWithComputed.InvokeCount);
            store.Value = 55;
            store.ComputedMethodIncerceptingValue(); invokeCount++;
            store.ComputedMethodIncerceptingValue();
            store.ComputedMethodIncerceptingValue();
            store.ComputedMethodIncerceptingValue();

            // Assert
            Assert.AreEqual(invokeCount, ClassStoreWithComputed.InvokeCount);
        }
    }
}
