//#define ENABLE_CACHING

using Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Interfaces;
using Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests
{
    [TestClass]
    public class RuntimeTypeBuilderTests
    {
        [TestMethod]
        public void BuildRuntimeType_ReadonlyProperties_Override()
        {
            // Arrange
            string value = "test value";

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(IInterfaceWithReadonlyProperty.ReadonlyStringProperty)))
                .Returns(value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(IInterfaceWithReadonlyProperty), getMethod, setMethod);
            IInterfaceWithReadonlyProperty impl = (IInterfaceWithReadonlyProperty)Activator.CreateInstance(runtimeType, manager);
            string returnedValue = impl.ReadonlyStringProperty;

            // Assert
            Assert.AreEqual(value, returnedValue);
        }

        [TestMethod]
        public void BuildRuntimeType_DefaultProperties_DontOverride()
        {
            // Arrange
            string value = "test value";

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(IInterfaceWithDefaultProperty.StringProperty)))
                .Returns(value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(IInterfaceWithDefaultProperty), getMethod, setMethod);
            IInterfaceWithDefaultProperty impl = (IInterfaceWithDefaultProperty)Activator.CreateInstance(runtimeType, manager);
            string returnedValue = impl.DefaultProperty;

            // Assert
            Assert.AreEqual(value, returnedValue);
        }

        [TestMethod]
        public void BuildRuntimeType_TestType_IncludeInheritedProperties()
        {
            // Arrange
            string value = "test value";

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(IInterfaceWithAncestor.ReferenceType)))
                .Returns(value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(IInterfaceWithAncestor), getMethod, setMethod);
            IInterfaceWithAncestor impl = (IInterfaceWithAncestor)Activator.CreateInstance(runtimeType, manager);

            // Assert
            Assert.AreEqual(value, impl.ReferenceType);
        }

        [TestMethod]
        public void BuildRuntimeType_ValueType_GetValue()
        {
            // Arrange
            int value = 50;

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(IClassicInterface.ValueType)))
                .Returns(value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(IClassicInterface), getMethod, setMethod);
            IClassicInterface impl = (IClassicInterface)Activator.CreateInstance(runtimeType, manager);

            // Assert
            Assert.AreEqual(value, impl.ValueType);
        }

        [TestMethod]
        public void BuildRuntimeType_ValueType_SetValue()
        {
            // Arrange
            int value = 50;
            object setValue = null;
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(IClassicInterface.ValueType)))
                .Returns(() => setValue);
            managerMock.Setup(x => x.SetValue(nameof(IClassicInterface.ValueType), It.IsAny<object>()))
                .Callback<string, object>((name, value) => setValue = value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(IClassicInterface), getMethod, setMethod);
            IClassicInterface impl = (IClassicInterface)Activator.CreateInstance(runtimeType, manager);
            impl.ValueType = value;

            // Assert
            Assert.AreEqual(value, impl.ValueType);
        }

        [TestMethod]
        public void BuildRuntimeType_ReferenceType_GetValue()
        {
            // Arrange
            string value = "test value";

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(IClassicInterface.ReferenceType)))
                .Returns(value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(IClassicInterface), getMethod, setMethod);
            IClassicInterface impl = (IClassicInterface)Activator.CreateInstance(runtimeType, manager);
            string returnedValue = impl.ReferenceType;

            // Assert
            Assert.AreEqual(value, returnedValue);
        }

        [TestMethod]
        public void BuildRuntimeType_ReferenceType_SetValue()
        {
            // Arrange
            string value = "test value";
            object setValue = null;
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.SetupGet(x => x.Implementation).Returns(null);
            managerMock.Setup(x => x.GetValue(nameof(IClassicInterface.ReferenceType)))
                .Returns(() => setValue);
            managerMock.Setup(x => x.SetValue(nameof(IClassicInterface.ReferenceType), It.IsAny<object>()))
                .Callback<string, object>((name, value) => setValue = value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(IClassicInterface), getMethod, setMethod);
            IClassicInterface impl = (IClassicInterface)Activator.CreateInstance(runtimeType, manager);
            impl.ReferenceType = value;
            
            // Assert
            Assert.AreEqual(value, impl.ReferenceType);
        }

        [TestMethod]
        public void BuildRuntimeType_Struct_GetValue()
        {
            // Arrange
            DefaultStruct defaultStruct = new DefaultStruct
            {
                ValueType = 50,
                ReferencType = "test data"
            };

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(InterfaceWithDefaultStruct.DefaultStruct)))
                .Returns(defaultStruct);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(InterfaceWithDefaultStruct), getMethod, setMethod);
            InterfaceWithDefaultStruct impl = (InterfaceWithDefaultStruct)Activator.CreateInstance(runtimeType, manager);

            // Assert
            Assert.AreEqual(defaultStruct, impl.DefaultStruct);
        }

        [TestMethod]
        public void BuildRuntimeType_Struct_SetValue()
        {
            // Arrange
            DefaultStruct defaultStruct = new DefaultStruct
            {
                ValueType = 50,
                ReferencType = "test data"
            };

            object setValue = null;

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(InterfaceWithDefaultStruct.DefaultStruct)))
                .Returns(() => setValue);
            managerMock.Setup(x => x.SetValue(nameof(InterfaceWithDefaultStruct.DefaultStruct), It.IsAny<object>()))
                .Callback<string, object>((name, value) => setValue = value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(InterfaceWithDefaultStruct), getMethod, setMethod);
            InterfaceWithDefaultStruct impl = (InterfaceWithDefaultStruct)Activator.CreateInstance(runtimeType, manager);
            impl.DefaultStruct = defaultStruct;

            // Assert
            Assert.AreEqual(defaultStruct, impl.DefaultStruct);
        }

#if ENABLE_CACHING
        [TestMethod]
        public void BuildRuntimeType_CacheType()
        {
            // Arrange
            var managerMock = new Mock<IMockableRuntimeTypePropertyManager>();
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            var managerMock2 = new Mock<IMockableRuntimeTypePropertyManager>();
            var manager2 = managerMock2.Object;
            MethodInfo getMethod2 = manager2.GetType().GetMethod("GetValue");
            MethodInfo setMethod2 = manager2.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeTypeBuilder.BuildRuntimeType(typeof(IClassicInterface), getMethod, setMethod);
            IClassicInterface impl = (IClassicInterface)Activator.CreateInstance(runtimeType, manager);

            Type runtimeType2 = RuntimeTypeBuilder.BuildRuntimeType(typeof(IClassicInterface), getMethod2, setMethod2);
            IClassicInterface impl2 = (IClassicInterface)Activator.CreateInstance(runtimeType2, manager2);

            // Assert
            Assert.AreEqual(impl.GetType(), impl2.GetType());
        }
#endif
    }
}
