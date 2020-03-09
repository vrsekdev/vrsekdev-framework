using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class DynamicStateProperty : DynamicObject, IObservable<PropertyAccessedArgs>, IDisposable
    {
        private readonly Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer> observers = new Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer>();

        private readonly Dictionary<string, DynamicObservableArray> observedDynamicArrays = new Dictionary<string, DynamicObservableArray>();
        private readonly Dictionary<string, object> observedDynamicProperties = new Dictionary<string, object>();

        internal ObservableProperty ObservableProperty { get; }
        internal ObservableFactory ObservableFactory { get; }

        public static DynamicStateProperty Create(ObservableProperty observableProperty)
        {
            return new DynamicStateProperty(observableProperty);
        }

        public static T Box<T>(DynamicStateProperty dynamicState)
            where T : class
        {
            return ImpromptuInterface.Impromptu.ActLike<T>(dynamicState);
        }

        public static dynamic Box(DynamicStateProperty dynamicState, Type type)
        {
            return ImpromptuInterface.Impromptu.ActLike(dynamicState, type);
        }

        public static DynamicStateProperty Unbox<T>(T val)
            where T : class
        {
            return ImpromptuInterface.Impromptu.UndoActLike(val) as DynamicStateProperty;
        }

        public static bool IsObservable(object value)
        {
            return Unbox(value) != null;
        }

        private DynamicStateProperty(ObservableProperty observableProperty)
        {
            ObservableProperty = observableProperty;
            ObservableFactory = ObservableProperty.CreateFactoryFrom(observableProperty);

            Initialize();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;

            OnPropertyAccessed(name);
            
            if (observedDynamicProperties.ContainsKey(name))
            {
                result = observedDynamicProperties[name];

                return true;
            }

            if (observedDynamicArrays.ContainsKey(name))
            {
                result = observedDynamicArrays[name];

                return true;
            }
            
            return ObservableProperty.TryGetMember(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name;

            if (observedDynamicArrays.TryGetValue(name, out DynamicObservableArray dynamicObservableArray))
            {
                bool result;
                if (result = ObservableProperty.TrySetMember(name, value))
                {
                    dynamicObservableArray.OverwriteElements(null);
                }

                return result;
            }

            return ObservableProperty.TrySetMember(name, value);
        }

        public IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer)
        {
            var disposer = new ObserverDisposer();

            foreach (var observedDynamicObject in observedDynamicProperties.Values)
            {
                DynamicStateProperty dynamicState = Unbox(observedDynamicObject);
                if (dynamicState != null)
                {
                    disposer.AddDisposeAction(dynamicState.Subscribe(observer));
                }
            }

            foreach (var observedDynamicArray in observedDynamicArrays.Values)
            {
                disposer.AddDisposeAction(observedDynamicArray.Subscribe(observer));
            }

            disposer.AddDisposeAction(() => observers.Remove(observer));

            observers.Add(observer, disposer);
            return disposer;
        }

        public override string ToString()
        {
            return ObservableProperty.ToString();
        }

        private void Initialize()
        {
            var observedProperties = ObservableProperty.GetObservedProperties();
            var observedArrays = ObservableProperty.GetObservedArrays();

            foreach (var observedProperty in observedProperties)
            {
                observedDynamicProperties[observedProperty.Key] = CreateObserverProperty(observedProperty.Value);
            }

            foreach (var observedArray in observedArrays)
            {
                observedDynamicArrays[observedArray.Key] = CreateObserverArray(observedArray.Value);
            }
        }

        private object CreateObserverProperty(ObservableProperty observableProperty)
        {
            DynamicStateProperty dynamicState = new DynamicStateProperty(observableProperty);
            object boxedItem = Box(dynamicState, observableProperty.ObservedType).Target;

            foreach (var observer in observers)
            {
                observer.Value.AddDisposeAction(dynamicState.Subscribe(observer.Key));
            }

            return boxedItem;
        }

        private DynamicObservableArray CreateObserverArray(ObservableArrayInternal observableArray)
        {
            Type elementType = observableArray.ElementType;
            Type dynamicObserverArrayType = typeof(DynamicObservableArray<>).MakeGenericType(elementType);

            DynamicObservableArray dynamicObservableArray = (DynamicObservableArray)Activator.CreateInstance(
                dynamicObserverArrayType, 
                observableArray, 
                ObservableFactory);

            foreach (var observer in observers)
            {
                observer.Value.AddDisposeAction(dynamicObservableArray.Subscribe(observer.Key));
            }

            return dynamicObservableArray;
        }

        private void OnPropertyAccessed(string name)
        {
            var args = new PropertyAccessedArgs
            {
                DynamicStateProperty = this,
                PropertyName = name
            };

            foreach (var observer in observers.Keys.ToList())
            {
                observer.OnNext(args);
            }
        }

        public void Dispose()
        {
            foreach (var observedDynamicProperty in observedDynamicProperties.Values)
            {
                DynamicStateProperty dynamicState = Unbox(observedDynamicProperty);
                dynamicState.Dispose();
            }

            foreach (var observedDynamicArray in observedDynamicArrays.Values)
            {
                observedDynamicArray.Dispose();
            }

            foreach (var observer in observers.Keys)
            {
                observer.OnCompleted();
            }
        }
    }
}
