using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class PropertyAccessedArgs
    {
        public ObservableProperty ObservableProperty { get; set; }

        public string PropertyName { get; set; }
    }

    internal class DynamicStateProperty : DynamicObject, IObservable<PropertyAccessedArgs>, IDisposable
    {
        private readonly Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer> observers = new Dictionary<IObserver<PropertyAccessedArgs>, ObserverDisposer>();

        private readonly Dictionary<string, DynamicObservableArray> observedDynamicArrays = new Dictionary<string, DynamicObservableArray>();
        private readonly Dictionary<string, object> observedDynamicProperties = new Dictionary<string, object>();

        internal ObservableProperty ObservableProperty { get; }

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

            if (observedDynamicArrays.ContainsKey(name))
            {
                foreach (object item in observedDynamicArrays[name])
                {
                    DynamicStateProperty dynamicState = Unbox(item);
                    if (dynamicState != null)
                    {
                        dynamicState.Dispose();
                    }
                }
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
                observedDynamicProperties[observedProperty.Key] = GetObserverProperty(observedProperty.Value);
            }

            foreach (var observedArray in observedArrays)
            {
                observedDynamicArrays[observedArray.Key] = GetObserverArray(observedArray.Value);
            }
        }

        private object GetObserverProperty(ObservableProperty observableProperty)
        {
            DynamicStateProperty dynamicState = new DynamicStateProperty(observableProperty);
            return Box(dynamicState, observableProperty.ObservedType).Target;
        }

        private DynamicObservableArray GetObserverArray(ObservableArrayInternal observableArray)
        {
            Type elementType = observableArray.ObservedElementType;
            Type dynamicObserverArrayType = typeof(DynamicObservableArray<>).MakeGenericType(elementType);

            return (DynamicObservableArray)Activator.CreateInstance(dynamicObserverArrayType, observableArray);
        }

        private void OnPropertyAccessed(string name)
        {
            var args = new PropertyAccessedArgs
            {
                ObservableProperty = ObservableProperty,
                PropertyName = name
            };

            foreach (var observer in observers.Keys)
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

            foreach (var observer in observers.Keys)
            {
                observer.OnCompleted();
            }
        }

        private class ObserverDisposer : IDisposable
        {
            private readonly List<Action> disposeActions = new List<Action>();

            private bool disposed;

            public void AddDisposeAction(Action action)
            {
                disposeActions.Add(action);
            }

            public void AddDisposeAction(IDisposable disposable)
            {
                disposeActions.Add(() => disposable.Dispose());
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    foreach (var disposeAction in disposeActions)
                    {
                        disposeAction();
                    }

                    disposed = true;
                }
#if DEBUG
                else
                {
                    throw new Exception("Already disposed.");
                }
#endif

            }
        }
    }
}
