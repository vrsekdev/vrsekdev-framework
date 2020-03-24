using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Proxies.RuntimeProxy.Emit;
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
        public void BuildRuntimeType_MethodInterceptors_Function_ProvideOwnInterceptedTargetInstance()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            ClassWithVirtualMethod expectedThisValue = new ClassWithVirtualMethod();
            Func<Func<ClassWithVirtualMethod, ClassWithVirtualMethod>, ClassWithVirtualMethod> interceptor = baseMethod => baseMethod(expectedThisValue);

            MethodInterceptions interceptions = new MethodInterceptions
            {
                Interceptions = new MethodInterception[]
                {
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.FunctionReturningThis)),
                        Delegate = interceptor,
                        ProvideInterceptedTarget = false
                    }
                }
            };

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptions });
            ClassWithVirtualMethod returnedValue = impl.FunctionReturningThis();
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
            Assert.AreEqual(expectedThisValue, returnedValue);
        }

        [TestMethod]
        public void BuildRuntimeType_MethodInterceptors_Function_MultipleParameters_InterceptClassMethod()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            int expectedValue = 50;
            Func<Func<string, int>, int> interceptor = (Func<string, int> baseMethod) => expectedValue;

            MethodInterceptions interceptions = new MethodInterceptions
            {
                Interceptions = new MethodInterception[]
                {
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.FunctionWithParameterToIntercept)),
                        Delegate = interceptor
                    }
                }
            };

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptions });
            int returnedValue = impl.FunctionWithParameterToIntercept(String.Empty);
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
            Assert.AreEqual(expectedValue, returnedValue);
        }

        [TestMethod]
        public void BuildRuntimeType_MethodInterceptors_Function_InterceptClassMethod_ReturnParentValue()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            string expectedValue = "base";
            Func<Func<string>, string> interceptor = baseMethod => baseMethod();

            MethodInterceptions interceptions = new MethodInterceptions
            {
                Interceptions = new MethodInterception[]
                {
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.FunctionToIntercept)),
                        Delegate = interceptor
                    }
                }
            };

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptions });
            string returnedValue = impl.FunctionToIntercept();
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
            Assert.AreEqual(expectedValue, returnedValue);
        }

        [TestMethod]
        public void BuildRuntimeType_MethodInterceptors_Function_InterceptClassMethod()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            string expectedValue = "interceptor";
            Func<Func<string>, string> interceptor = baseMethod => expectedValue;

            MethodInterceptions interceptions = new MethodInterceptions
            {
                Interceptions = new MethodInterception[]
                {
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.FunctionToIntercept)),
                        Delegate = interceptor
                    }
                }
            };

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptions });
            string returnedValue = returnedValue = impl.FunctionToIntercept();
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
            Assert.AreEqual(expectedValue, returnedValue);
        }

        [TestMethod]
        public void BuildRuntimeType_MethodInterceptors_InterceptMultipleClassMethods()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            bool interceptorCalled = false;
            Action<Action> interceptor = baseAction => interceptorCalled = true;
            bool interceptor2Called = false;
            Action<Action> interceptor2 = baseAction => interceptor2Called = true;

            MethodInterceptions interceptions = new MethodInterceptions
            {
                Interceptions = new MethodInterception[]
                {
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.ActionToIntercept)),
                        Delegate = interceptor
                    },
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.ActionToIntercept2)),
                        Delegate = interceptor2
                    }
                }
            };

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptions });
            impl.ActionToIntercept();
            impl.ActionToIntercept2();
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
            Assert.IsTrue(interceptorCalled);
            Assert.IsTrue(interceptor2Called);
        }

        [TestMethod]
        public void BuildRuntimeType_MethodInterceptors_CallBase()
        {
            // Arrange
            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            Action<Action> interceptor = baseAction => baseAction();
            MethodInterceptions interceptions = new MethodInterceptions
            {
                Interceptions = new MethodInterception[]
                {
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.ActionToIntercept)),
                        Delegate = interceptor
                    }
                }
            };

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptions });
            impl.ActionToIntercept();
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
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
            Action<Action> interceptor = (Action baseAction) => interceptorCalled = true;

            MethodInterceptions interceptions = new MethodInterceptions
            {
                Interceptions = new MethodInterception[]
                {
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(ClassWithVirtualMethod).GetMethod(nameof(ClassWithVirtualMethod.ActionToIntercept)),
                        Delegate = interceptor
                    }
                }
            };

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithVirtualMethod), getMethod, setMethod, interceptions);
            ClassWithVirtualMethod impl = (ClassWithVirtualMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptions });
            impl.ActionToIntercept();
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Action interceptor = () => interceptorCalled = true;

            MethodInterceptions interceptions = new MethodInterceptions
            {
                Interceptions = new MethodInterception[]
                {
                    new DelegateMethodInterception
                    {
                        InterceptedMethod = typeof(IInterfaceWithMethod).GetMethod(nameof(IInterfaceWithMethod.MethodToIntercept)),
                        Delegate = interceptor
                    }
                }
            };

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(IInterfaceWithMethod), getMethod, setMethod, interceptions);
            IInterfaceWithMethod impl = (IInterfaceWithMethod)Activator.CreateInstance(runtimeType, new object[] { manager, interceptions });
            impl.MethodToIntercept();
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(IInterfaceWithReadonlyProperty), getMethod, setMethod);
            IInterfaceWithReadonlyProperty impl = (IInterfaceWithReadonlyProperty)Activator.CreateInstance(runtimeType, manager);
            string returnedValue = impl.ReadonlyStringProperty;
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
            Assert.AreEqual(value, returnedValue);
        }

        [TestMethod]
        public void BuildRuntimeType_Property_PreserveDefaultValue()
        {
            string propertyName = nameof(ClassWithDefaultValue.PropertyWithDefaultValue);
            string defaultValue = new ClassWithDefaultValue().PropertyWithDefaultValue;
            bool setDefaultInvoked = false;

            Mock<IMockableRuntimeTypePropertyManager> managerMock = new Mock<IMockableRuntimeTypePropertyManager>(MockBehavior.Strict);
            managerMock.Setup(x => x.GetValue(propertyName))
                .Returns(null);
            managerMock.Setup(x => x.SetDefaultValue(propertyName, defaultValue)).Callback(() => setDefaultInvoked = true);

            var manager = managerMock.Object;
            MethodInfo getMethod = manager.GetType().GetMethod("GetValue");
            MethodInfo setMethod = manager.GetType().GetMethod("SetValue");

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ClassWithDefaultValue), getMethod, setMethod);
            ClassWithDefaultValue impl = (ClassWithDefaultValue)Activator.CreateInstance(runtimeType, manager);
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
            Assert.IsTrue(setDefaultInvoked);
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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(IInterfaceWithDefaultProperty), getMethod, setMethod);
            IInterfaceWithDefaultProperty impl = (IInterfaceWithDefaultProperty)Activator.CreateInstance(runtimeType, manager);
            string returnedValue = impl.DefaultProperty;
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(IInterfaceWithAncestor), getMethod, setMethod);
            IInterfaceWithAncestor impl = (IInterfaceWithAncestor)Activator.CreateInstance(runtimeType, manager);
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ISimpleInterface), getMethod, setMethod);
            ISimpleInterface impl = (ISimpleInterface)Activator.CreateInstance(runtimeType, manager);
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ISimpleInterface), getMethod, setMethod);
            ISimpleInterface impl = (ISimpleInterface)Activator.CreateInstance(runtimeType, manager);
            impl.ValueTypeValue = value;
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ISimpleInterface), getMethod, setMethod);
            ISimpleInterface impl = (ISimpleInterface)Activator.CreateInstance(runtimeType, manager);
            string returnedValue = impl.StringValue;
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(ISimpleInterface), getMethod, setMethod);
            ISimpleInterface impl = (ISimpleInterface)Activator.CreateInstance(runtimeType, manager);
            impl.StringValue = value;
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(InterfaceWithSimpleStruct), getMethod, setMethod);
            InterfaceWithSimpleStruct impl = (InterfaceWithSimpleStruct)Activator.CreateInstance(runtimeType, manager);
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

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
            Stopwatch sw = Stopwatch.StartNew();
            Type runtimeType = RuntimeProxyBuilder.BuildRuntimeType(typeof(InterfaceWithSimpleStruct), getMethod, setMethod);
            InterfaceWithSimpleStruct impl = (InterfaceWithSimpleStruct)Activator.CreateInstance(runtimeType, manager);
            impl.DefaultStruct = defaultStruct;
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);

            // Assert
            Assert.AreEqual(defaultStruct, impl.DefaultStruct);
        }
    }
}
