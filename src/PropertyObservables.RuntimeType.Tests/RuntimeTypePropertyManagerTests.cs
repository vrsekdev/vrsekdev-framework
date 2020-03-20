using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Fakes;
using Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.PropertyObservables.RuntimeType.Tests
{
    [TestClass]
    public class RuntimeTypePropertyManagerTests
    {
        [TestMethod]
        public void Implementation_MultipleManagers_DifferentInterfaces()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            Mock<IObserver<PropertyAccessedArgs>> observerMock = new Mock<IObserver<PropertyAccessedArgs>>();

            // Act
            var manager = new RuntimeTypePropertyObservableManager<ISimpleInterface>(observableMock.Object);
            var manager2 = new RuntimeTypePropertyObservableManager<IInterfaceWithNestedObservable>(observableMock.Object);

            manager.Subscribe(observerMock.Object);
            manager2.Subscribe(observerMock.Object);

            ISimpleInterface impl = manager.Implementation;
            IInterfaceWithNestedObservable impl2 = manager2.Implementation;

            Trace.WriteLine(impl.StringValue);
            Trace.WriteLine(impl2.NestedObservable);

            // Assert
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(ISimpleInterface.StringValue))), Times.Once);
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(IInterfaceWithNestedObservable.NestedObservable))), Times.Once);
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

            Mock<IObserver<PropertyAccessedArgs>> observerMock = new Mock<IObserver<PropertyAccessedArgs>>();

            // Act
            var manager = new RuntimeTypePropertyObservableManager<IInterfaceWithNestedObservable>(observableMock.Object);
            manager.Subscribe(observerMock.Object);
            IInterfaceWithNestedObservable impl = manager.Implementation;
            Trace.WriteLine(impl.NestedObservable.StringValue);

            // Assert
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(IInterfaceWithNestedObservable.NestedObservable))), Times.Once);
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(ISimpleInterface.StringValue))), Times.Once);
        }

        [TestMethod]
        public void Implementation_InterfaceWithDefaultProperty_NotifyUnderlyingPropertyAccessed()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            Mock<IObserver<PropertyAccessedArgs>> observerMock = new Mock<IObserver<PropertyAccessedArgs>>();

            // Act
            var manager = new RuntimeTypePropertyObservableManager<IInterfaceWithDefaultProperty>(observableMock.Object);
            manager.Subscribe(observerMock.Object);
            IInterfaceWithDefaultProperty impl = manager.Implementation;
            Trace.WriteLine(impl.DefaultProperty);

            // Assert
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.StringProperty))), Times.Once);
        }

        [TestMethod]
        public void Implementation_InterfaceWithReadonlyProperty_NotifyPropertyAccessed()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());
            observableMock.Setup(x => x.TryGetMember(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            Mock<IObserver<PropertyAccessedArgs>> observerMock = new Mock<IObserver<PropertyAccessedArgs>>();

            // Act
            var manager = new RuntimeTypePropertyObservableManager<IInterfaceWithReadonlyProperty>(observableMock.Object);
            manager.Subscribe(observerMock.Object);
            IInterfaceWithReadonlyProperty impl = manager.Implementation;
            Trace.WriteLine(impl.ReadonlyStringProperty);

            // Assert
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.ReadonlyStringProperty))), Times.Once);
        }

        [TestMethod]
        public void Implementation_ValueType_NotifyPropertyAccessed()
        {
            // Arrange
            FakeObservableProperty observableProperty = new FakeObservableProperty();
            observableProperty.TrySetMember(nameof(ISimpleInterface.ValueTypeValue), 10);
            Mock<IObserver<PropertyAccessedArgs>> observerMock = new Mock<IObserver<PropertyAccessedArgs>>();

            // Act
            var manager = new RuntimeTypePropertyObservableManager<ISimpleInterface>(observableProperty);
            manager.Subscribe(observerMock.Object);
            ISimpleInterface impl = manager.Implementation;
            Trace.WriteLine(impl.ValueTypeValue);

            // Assert
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.ValueTypeValue))), Times.Once);
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

            Mock<IObserver<PropertyAccessedArgs>> observerMock = new Mock<IObserver<PropertyAccessedArgs>>();
            // Act
            var manager = new RuntimeTypePropertyObservableManager<ISimpleInterface>(observableMock.Object);
            manager.Subscribe(observerMock.Object);
            ISimpleInterface impl = manager.Implementation;
            Trace.WriteLine(impl.StringValue);

            // Assert
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.StringValue))), Times.Once);
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

            Mock<IObserver<PropertyAccessedArgs>> observerMock = new Mock<IObserver<PropertyAccessedArgs>>();
            // Act
            var manager = new RuntimeTypePropertyObservableManager<ISimpleInterface>(observableMock.Object);
            manager.Subscribe(observerMock.Object);
            ISimpleInterface impl = manager.Implementation;
            Trace.WriteLine(impl.ReferenceTypeValue);

            // Assert
            observerMock.Verify(x => x.OnNext(It.Is<PropertyAccessedArgs>(args => args.PropertyName == nameof(impl.ReferenceTypeValue))), Times.Once);
        }

        [TestMethod]
        public void Implementation_Manager_IsCorrectInstance()
        {
            // Arrange
            Mock<IObservableProperty> observableMock = new Mock<IObservableProperty>();
            observableMock.Setup(x => x.GetObservedProperties()).Returns(new Dictionary<string, IObservableProperty>());
            observableMock.Setup(x => x.GetObservedCollections()).Returns(new Dictionary<string, IObservableCollection>());

            Mock<IObserver<PropertyAccessedArgs>> observerMock = new Mock<IObserver<PropertyAccessedArgs>>();
            // Act
            var manager = new RuntimeTypePropertyObservableManager<ISimpleInterface>(observableMock.Object);
            ISimpleInterface impl = manager.Implementation;

            // Assert
            Assert.AreEqual(manager, ((IRuntimeTypeImpl)impl).Manager);
        }
    }
}
