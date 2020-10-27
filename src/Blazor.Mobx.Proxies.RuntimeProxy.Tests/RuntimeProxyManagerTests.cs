using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Exceptions;
using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Fakes;
using VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Proxies.RuntimeProxy.Tests
{
    [TestClass]
    public class RuntimeProxyManagerTests
    {
        [TestMethod]
        public void Implementation_RuntimeTypeWithoutInterceptions_CacheRuntimeType()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            Mock<IObservableProperty> nestedObservableMock = new Mock<IObservableProperty>();
            nestedObservableMock.SetupGet(x => x.ObservedType).Returns(typeof(ISimpleInterface));
            nestedObservableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            nestedObservableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            nestedObservableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);
            var nestedObservable = (object)nestedObservableMock.Object;

            Mock<IObservableProperty> observableMock2 = new Mock<IObservableProperty>();
            observableMock2.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>
            {
                [nameof(IInterfaceWithNestedObservable.NestedObservable)] = nestedObservableMock.Object
            });
            observableMock2.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock2.Setup(x => x.TryGetMember(It.IsAny<string>(), out nestedObservable)).Returns(true);

            // Act
            var manager = new RuntimeProxyManager<ISimpleInterface>(observableMock.Object);
            var manager2 = new RuntimeProxyManager<IInterfaceWithNestedObservable>(observableMock2.Object);

            ISimpleInterface impl = manager.Implementation;
            IInterfaceWithNestedObservable impl2 = manager2.Implementation;

            Trace.WriteLine(impl.StringValue);
            Trace.WriteLine(impl2.NestedObservable);

            // Assert
            Assert.AreEqual(impl.GetType(), impl2.NestedObservable.GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(PropertyReadonlyException))]
        public void Implementation_IsReadonly_ObservedPropertyShouldThrowException()
        {
            // Arrange
            Mock<IObservableProperty> nestedObservableMock = new Mock<IObservableProperty>();
            nestedObservableMock.SetupGet(x => x.ObservedType).Returns(typeof(ISimpleInterface));
            nestedObservableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            nestedObservableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            nestedObservableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);
            var nestedObservable = (object)nestedObservableMock.Object;

            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>
            {
                [nameof(IInterfaceWithNestedObservable.NestedObservable)] = nestedObservableMock.Object
            });
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out nestedObservable)).Returns(true);
            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();

            // Act
            var manager = new RuntimeProxyManager<IInterfaceWithNestedObservable>(observableMock.Object, true);
            manager.Subscribe(subscriberMock.Object);

            IInterfaceWithNestedObservable impl = manager.Implementation;

            impl.NestedObservable.StringValue = "test";

            // Assert
            // exc
        }

        [TestMethod]
        [ExpectedException(typeof(PropertyReadonlyException))]
        public void Implementation_IsReadonly_ThrowException()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);
            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();

            // Act
            var manager = new RuntimeProxyManager<ISimpleInterface>(observableMock.Object, true);
            manager.Subscribe(subscriberMock.Object);

            ISimpleInterface impl = manager.Implementation;

            impl.StringValue = "test";

            // Assert
            // exc
        }

        [TestMethod]
        public void Implementation_MultipleManagers_DifferentInterfaces()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();

            // Act
            var manager = new RuntimeProxyManager<ISimpleInterface>(observableMock.Object);
            var manager2 = new RuntimeProxyManager<IInterfaceWithNestedObservable>(observableMock.Object);

            manager.Subscribe(subscriberMock.Object);
            manager2.Subscribe(subscriberMock.Object);

            ISimpleInterface impl = manager.Implementation;
            IInterfaceWithNestedObservable impl2 = manager2.Implementation;

            Trace.WriteLine(impl.StringValue);
            Trace.WriteLine(impl2.NestedObservable);

            // Assert
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(ISimpleInterface.StringValue))), Times.Once);
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(IInterfaceWithNestedObservable.NestedObservable))), Times.Once);
        }

        [TestMethod]
        public void Implementation_NestedObservedProperty_NotifyAllPropertyAccessed()
        {
            // Arrange
            Mock<IObservableProperty> nestedObservableMock = new Mock<IObservableProperty>();
            nestedObservableMock.SetupGet(x => x.ObservedType).Returns(typeof(ISimpleInterface));
            nestedObservableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            nestedObservableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            nestedObservableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);
            var nestedObservable = (object)nestedObservableMock.Object;

            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>
            {
                [nameof(IInterfaceWithNestedObservable.NestedObservable)] = nestedObservableMock.Object
            });
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(nameof(IInterfaceWithNestedObservable.NestedObservable), out nestedObservable)).Returns(true);

            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();

            // Act
            var manager = new RuntimeProxyManager<IInterfaceWithNestedObservable>(observableMock.Object);
            manager.Subscribe(subscriberMock.Object);
            IInterfaceWithNestedObservable impl = manager.Implementation;
            Trace.WriteLine(impl.NestedObservable.StringValue);

            // Assert
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(IInterfaceWithNestedObservable.NestedObservable))), Times.Once);
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(ISimpleInterface.StringValue))), Times.Once);
        }

        [TestMethod]
        public void Implementation_InterfaceWithDefaultProperty_NotifyUnderlyingPropertyAccessed()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();

            // Act
            var manager = new RuntimeProxyManager<IInterfaceWithDefaultProperty>(observableMock.Object);
            manager.Subscribe(subscriberMock.Object);
            IInterfaceWithDefaultProperty impl = manager.Implementation;
            Trace.WriteLine(impl.DefaultProperty);

            // Assert
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.StringProperty))), Times.Once);
        }

        [TestMethod]
        public void Implementation_InterfaceWithReadonlyProperty_NotifyPropertyAccessed()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();

            // Act
            var manager = new RuntimeProxyManager<IInterfaceWithReadonlyProperty>(observableMock.Object);
            manager.Subscribe(subscriberMock.Object);
            IInterfaceWithReadonlyProperty impl = manager.Implementation;
            Trace.WriteLine(impl.ReadonlyStringProperty);

            // Assert
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.ReadonlyStringProperty))), Times.Once);
        }

        [TestMethod]
        public void Implementation_ValueType_NotifyPropertyAccessed()
        {
            // Arrange
            object value = 10;
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(nameof(ISimpleInterface.ValueTypeValue), out value)).Returns(true);
            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();

            // Act
            var manager = new RuntimeProxyManager<ISimpleInterface>(observableMock.Object);
            manager.Subscribe(subscriberMock.Object);
            ISimpleInterface impl = manager.Implementation;
            Trace.WriteLine(impl.ValueTypeValue);

            // Assert
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.ValueTypeValue))), Times.Once);
        }

        [TestMethod]
        public void Implementation_StringValue_NotifyPropertyAccessed()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            object outParam = "tesada";
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out outParam)).Returns(true);

            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();
            // Act
            var manager = new RuntimeProxyManager<ISimpleInterface>(observableMock.Object);
            manager.Subscribe(subscriberMock.Object);
            ISimpleInterface impl = manager.Implementation;
            Trace.WriteLine(impl.StringValue);

            // Assert
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.StringValue))), Times.Once);
        }

        [TestMethod]
        public void Implementation_ReferenceTypeValue_NotifyPropertyAccessed()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            object outParam = "tesada";
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out outParam)).Returns(true);

            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();
            // Act
            var manager = new RuntimeProxyManager<ISimpleInterface>(observableMock.Object);
            manager.Subscribe(subscriberMock.Object);
            ISimpleInterface impl = manager.Implementation;
            Trace.WriteLine(impl.ReferenceTypeValue);

            // Assert
            subscriberMock.Verify(x => x.OnPropertyAccessed(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.ReferenceTypeValue))), Times.Once);
        }

        [TestMethod]
        public void Implementation_Manager_IsCorrectInstance()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());

            Mock<IPropertyAccessedSubscriber> subscriberMock = new Mock<IPropertyAccessedSubscriber>();
            // Act
            var manager = new RuntimeProxyManager<ISimpleInterface>(observableMock.Object);
            ISimpleInterface impl = manager.Implementation;

            // Assert
            Assert.AreEqual(manager, ((IRuntimeProxy)impl).Manager);
        }
    }
}
