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

    public class DynamicStateProperty : DynamicObject, IObservable<PropertyAccessedArgs>
    {
        private readonly List<IObserver<PropertyAccessedArgs>> observers = new List<IObserver<PropertyAccessedArgs>>();

        private readonly Dictionary<string, ObservableArray> observedDynamicArrays = new Dictionary<string, ObservableArray>();
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

        public static DynamicStateProperty Unbox<TState>(TState state)
            where TState : class
        {
            return ImpromptuInterface.Impromptu.UndoActLike(state) as DynamicStateProperty;
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
            return ObservableProperty.TrySetMember(binder.Name, value);
        }

        public IDisposable Subscribe(IObserver<PropertyAccessedArgs> observer)
        {
            observers.Add(observer);

            foreach (var observedDynamicObject in observedDynamicProperties)
            {
                DynamicStateProperty dynamicState = Unbox(observedDynamicObject.Value);
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
            return ImpromptuInterface.Impromptu.ActLike(new DynamicStateProperty(observableProperty), observableProperty.ObservedType).Target;
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
