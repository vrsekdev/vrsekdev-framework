using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Classes;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Interfaces;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests
{
    [TestClass]
    public class RuntimeProxyBuilderTests
    {
        [TestMethod]
        public void BuildRuntimeType_MethodInterceptors_InterceptClassMethodAndCallBase()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            bool interceptorCalled = false;
            MethodInterceptorProxy interceptorProxy = new MethodInterceptorProxy(() => interceptorCalled = true);

            MethodInterception[] interceptions = new MethodInterception[]
            {
                new MethodInterception
                {
                    InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.MethodToIntercept)),
                    Interceptor = typeof(MethodInterceptorProxy).GetMethod(nameof(MethodInterceptorProxy.Invoke))
                }
            };

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptorProxy });
            impl.MethodToIntercept();

            // Assert
            Assert.IsTrue(interceptorCalled);
            Assert.IsTrue(impl.InterceptedMethodCalled);
        }

        [TestMethod]
        public void BuildRuntimeType_MethodInterceptors_InterceptClassMethod()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            bool interceptorCalled = false;
            MethodInterceptorProxy interceptorProxy = new MethodInterceptorProxy(() => interceptorCalled = true);

            MethodInterception[] interceptions = new MethodInterception[]
            {
                new MethodInterception
                {
                    InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.MethodToIntercept)),
                    Interceptor = typeof(MethodInterceptorProxy).GetMethod(nameof(MethodInterceptorProxy.Invoke))
                }
            };

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptorProxy });
            impl.MethodToIntercept();

            // Assert
            Assert.IsTrue(interceptorCalled);
        }

        [TestMethod]
        public void BuildRuntimeType_MethodInterceptors_InterceptInterfaceMethod()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            bool interceptorCalled = false;
            MethodInterceptorProxy interceptorProxy = new MethodInterceptorProxy(() => interceptorCalled = true);

            MethodInterception[] interceptions = new MethodInterception[]
            {
                new MethodInterception
                {
                    InterceptedMethod = typeof(IInterfaceWithMethod).GetMethod(nameof(IInterfaceWithMethod.MethodToIntercept)),
                    Interceptor = typeof(MethodInterceptorProxy).GetMethod(nameof(MethodInterceptorProxy.Invoke))
                }
            };

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(IInterfaceWithMethod), getMethod, setMethod, interceptions);
            IInterfaceWithMethod impl = (IInterfaceWithMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptorProxy });
            impl.MethodToIntercept();

            // Assert
            Assert.IsTrue(interceptorCalled);
        }

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
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(IInterfaceWithReadonlyProperty), getMethod, setMethod);
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
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(IInterfaceWithDefaultProperty), getMethod, setMethod);
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
            managerMock.Setup(x => x.GetValue(nameof(IInterfaceWithAncestor.StringValue)))
                .Returns(value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(IInterfaceWithAncestor), getMethod, setMethod);
            IInterfaceWithAncestor impl = (IInterfaceWithAncestor)Activator.CreateInstance(runtimeType, manager);

            // Assert
            Assert.AreEqual(value, impl.StringValue);
        }

        [TestMethod]
        public void BuildRuntimeType_ValueType_GetValue()
        {
            // Arrange
            int value = 50;

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(ISimpleInterface.ValueTypeValue)))
                .Returns(value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ISimpleInterface), getMethod, setMethod);
            ISimpleInterface impl = (ISimpleInterface)Activator.CreateInstance(runtimeType, manager);

            // Assert
            Assert.AreEqual(value, impl.ValueTypeValue);
        }

        [TestMethod]
        public void BuildRuntimeType_ValueType_SetValue()
        {
            // Arrange
            int value = 50;
            object setValue = null;
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(ISimpleInterface.ValueTypeValue)))
                .Returns(() => setValue);
            managerMock.Setup(x => x.SetValue(nameof(ISimpleInterface.ValueTypeValue), It.IsAny<object>()))
                .Callback<string, object>((name, value) => setValue = value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ISimpleInterface), getMethod, setMethod);
            ISimpleInterface impl = (ISimpleInterface)Activator.CreateInstance(runtimeType, manager);
            impl.ValueTypeValue = value;

            // Assert
            Assert.AreEqual(value, impl.ValueTypeValue);
        }

        [TestMethod]
        public void BuildRuntimeType_ReferenceType_GetValue()
        {
            // Arrange
            string value = "test value";

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(ISimpleInterface.StringValue)))
                .Returns(value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ISimpleInterface), getMethod, setMethod);
            ISimpleInterface impl = (ISimpleInterface)Activator.CreateInstance(runtimeType, manager);
            string returnedValue = impl.StringValue;

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
            managerMock.Setup(x => x.GetValue(nameof(ISimpleInterface.StringValue)))
                .Returns(() => setValue);
            managerMock.Setup(x => x.SetValue(nameof(ISimpleInterface.StringValue), It.IsAny<object>()))
                .Callback<string, object>((name, value) => setValue = value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ISimpleInterface), getMethod, setMethod);
            ISimpleInterface impl = (ISimpleInterface)Activator.CreateInstance(runtimeType, manager);
            impl.StringValue = value;
            
            // Assert
            Assert.AreEqual(value, impl.StringValue);
        }

        [TestMethod]
        public void BuildRuntimeType_Struct_GetValue()
        {
            // Arrange
            SimpleStruct defaultStruct = new SimpleStruct
            {
                ValueType = 50,
                ReferencType = "test data"
            };

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(InterfaceWithSimpleStruct.DefaultStruct)))
                .Returns(defaultStruct);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(InterfaceWithSimpleStruct), getMethod, setMethod);
            InterfaceWithSimpleStruct impl = (InterfaceWithSimpleStruct)Activator.CreateInstance(runtimeType, manager);

            // Assert
            Assert.AreEqual(defaultStruct, impl.DefaultStruct);
        }

        [TestMethod]
        public void BuildRuntimeType_Struct_SetValue()
        {
            // Arrange
            SimpleStruct defaultStruct = new SimpleStruct
            {
                ValueType = 50,
                ReferencType = "test data"
            };

            object setValue = null;

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(nameof(InterfaceWithSimpleStruct.DefaultStruct)))
                .Returns(() => setValue);
            managerMock.Setup(x => x.SetValue(nameof(InterfaceWithSimpleStruct.DefaultStruct), It.IsAny<object>()))
                .Callback<string, object>((name, value) => setValue = value);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(InterfaceWithSimpleStruct), getMethod, setMethod);
            InterfaceWithSimpleStruct impl = (InterfaceWithSimpleStruct)Activator.CreateInstance(runtimeType, manager);
            impl.DefaultStruct = defaultStruct;

            // Assert
            Assert.AreEqual(defaultStruct, impl.DefaultStruct);
        }
    }
}
