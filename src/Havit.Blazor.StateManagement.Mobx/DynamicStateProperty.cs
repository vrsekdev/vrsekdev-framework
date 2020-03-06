using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Havit.Blazor.StateManagement.Mobx
{
    public class PropertyAccessedArgs
    {
        public ObservableProperty ObservableProperty { get; set; }

        public string PropertyName { get; set; }
    }

    public class DynamicStateProperty : DynamicObject, IObservable<PropertyAccessedArgs>, IDisposable
    {
        private readonly List<IObserver<PropertyAccessedArgs>> observers = new List<IObserver<PropertyAccessedArgs>>();

        private readonly Dictionary<string, ObservableArrayInternal> observedDynamicArrays = new Dictionary<string, ObservableArrayInternal>();
        private readonly Dictionary<string, object> observedDynamicProperties = new Dictionary<string, object>();

        internal ObservableProperty ObservableProperty { get; }

        public static DynamicStateProperty Create(ObservableProperty observableProperty)
        {
            return new DynamicStateProperty(observableProperty);
        }

        public static TState Box<TState>(DynamicStateProperty dynamicState)
            where TState : class
        {
            return ImpromptuInterface.Impromptu.ActLike<TState>(dynamicState);
        }

        public static object Box(DynamicStateProperty dynamicState, Type type)
        {
            return ImpromptuInterface.Impromptu.ActLike(dynamicState, type).Target;
        }

        public static DynamicStateProperty Unbox<TStore>(TStore store)
            where TStore : class
        {
            return ImpromptuInterface.Impromptu.UndoActLike(store) as DynamicStateProperty;
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
            
            return ObservableProperty.TryGetMember(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name;

            if (observedDynamicArrays.ContainsKey(name))
            {
                foreach (object item in observedDynamicArrays[name])
                {
                    DynamicStateProperty dynamicState = DynamicStateProperty.Unbox(item);
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
            observers.Add(observer);

            foreach (var observedDynamicObject in observedDynamicProperties.Values)
            {
                DynamicStateProperty dynamicState = Unbox(observedDynamicObject);
                if (dynamicState != null)
                {
                    dynamicState.Subscribe(observer);
                }
            }

            return new ObserverDisposer(() => observers.Remove(observer));
        }

        public override string ToString()
        {
            return ObservableProperty.ToString();
        }

        private void Initialize()
        {
            var observedProperties = ObservableProperty.GetObservedProperties();

            foreach (var observedProperty in observedProperties)
            {
                observedDynamicProperties[observedProperty.Key] = GetObserverProperty(observedProperty.Value);
            }
        }

        private object GetObserverProperty(ObservableProperty observableProperty)
        {
            DynamicStateProperty dynamicState = new DynamicStateProperty(observableProperty);
            return Box(dynamicState, observableProperty.ObservedType);
        }

        private void OnPropertyAccessed(string name)
        {
            var args = new PropertyAccessedArgs
            {
                ObservableProperty = ObservableProperty,
                PropertyName = name
            };

            foreach (var observer in observers)
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

            foreach (var observer in observers)
            {
                observer.OnCompleted();
            }
        }

        private class ObserverDisposer : IDisposable
        {
            private readonly Action disposeAction;

            public ObserverDisposer(Action disposeAction)
            {
                this.disposeAction = disposeAction;
            }

            public void Dispose()
            {
                disposeAction();
            }
        }
    }
}
