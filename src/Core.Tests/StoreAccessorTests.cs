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
            services.AddMobxStore<ClassStoreWithComputed>().LifestyleTransient();

            serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void Store_PropertyHasDefaultValue_PreserveDefaultValue()
        {
            string defaultValue = new ClassStoreWithDefaultValue().PropertyWithDefaultValue;

            IStoreAccessor<ClassStoreWithComputed> storeAccessor = serviceProvider.GetRequiredService<IStoreAccessor<ClassStoreWithComputed>>();
            var consumer = new FakeBlazorComponent();
            storeAccessor.SetConsumer(consumer);

            // Assert
            Assert.AreEqual(defaultValue, storeAccessor.Store.PropertyWithDefaultValue);
        }
    }
}
