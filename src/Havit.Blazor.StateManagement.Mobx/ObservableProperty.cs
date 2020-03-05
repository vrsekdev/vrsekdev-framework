using Havit.Blazor.StateManagement.Mobx.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    public class ObservableProperty : IObservable
    {
        private readonly EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent;
        private readonly EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private Dictionary<string, ObservableProperty> observedProperties;
        private Dictionary<string, ObservableArray> observedArrays;
        private Dictionary<string, PropertyInfo> allPropertiesByName;
        private Dictionary<string, object> normalProperties;

        public ObservableType ObservableType => ObservableType.Property;

        public Type ObservedType { get; }

        internal static ObservableProperty CreateCopy(ObservableProperty observableProperty)
        {
            var newObservableProperty = CreateEmptyCopy(observableProperty);
            newObservableProperty.OverwriteFrom(observableProperty);

            return newObservableProperty;
        }

        internal static ObservableProperty CreateEmptyCopy(ObservableProperty observableProperty)
        {
            var newObservableProperty = new ObservableProperty(
                observableProperty.ObservedType,
                observableProperty.statePropertyChangedEvent,
                observableProperty.collectionItemsChangedEvent);

            return newObservableProperty;
        }

        public ObservableProperty(
            Type interfaceType,
            EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent,
            EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Only interfaces can be observable.");
            }

            this.ObservedType = interfaceType;
            this.statePropertyChangedEvent = statePropertyChangedEvent;
            this.collectionItemsChangedEvent = collectionItemsChangedEvent;

            Initialize();
        }

        public bool TryGetMember(string name, out object result)
        {
            if (!allPropertiesByName.ContainsKey(name))
            {
                result = null;

                return false;
            }

            if (observedProperties.ContainsKey(name))
            {
                result = observedProperties[name];

                return true;
            }

            if (observedArrays.ContainsKey(name))
            {
                result = observedArrays[name];

                return true;
            }

            if (normalProperties.ContainsKey(name))
            {
                result = normalProperties[name];

                return true;
            }

            throw new Exception();
        }

        public bool TrySetMember(string name, object value)
        {
            if (!allPropertiesByName.ContainsKey(name))
            {
                return false;
            }

            if (observedProperties.ContainsKey(name))
            {
                observedProperties[name].OverwriteFrom(value);

                statePropertyChangedEvent?.Invoke(this, new StatePropertyChangedEventArgs
                {
                    PropertyName = name
                });

                return true;
            }

            if (observedArrays.ContainsKey(name))
            {
                if (!(value is IEnumerable<object> valueArray))
                {
                    throw new Exception("Not an array");
                }

                //var oldArray = observedArrays[name];
                var newArray = CreateObservableArray(allPropertiesByName[name], valueArray);
                observedArrays[name] = newArray;

                collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs(Enumerable.Empty<object>(), Enumerable.Empty<object>())
                {
                    OldCount = 0,
                    NewCount = 1
                });

                return true;
            }

            if (normalProperties.ContainsKey(name))
            {
                if (value != null && !allPropertiesByName[name].PropertyType.IsAssignableFrom(value.GetType()))
                {
                    return false;
                }

                normalProperties[name] = value;

                statePropertyChangedEvent?.Invoke(this, new StatePropertyChangedEventArgs
                {
                    PropertyName = name
                });

                return true;
            }

            throw new Exception();
        }

        public Dictionary<string, ObservableProperty> GetObservedProperties()
        {
            return observedProperties;
        }

        public Dictionary<string, ObservableArray> GetObservedArrays()
        {
            return observedArrays;
        }

        public void OverwriteFrom(object source)
        {
            if (source is ObservableProperty observableProperty)
            {
                OverwriteFrom(observableProperty);
                return;
            }

            foreach (var propertyKvp in allPropertiesByName)
            {
                if (!ObservedType.IsAssignableFrom(source.GetType()))
                {
                    throw new ArgumentException($"{nameof(source)} is not assignable from {ObservedType.Name}");
                }

                object newValue = propertyKvp.Value.GetValue(source);
                if (!TrySetMember(propertyKvp.Key, newValue))
                {
                    throw new Exception($"Could not copy to target. Property name: {propertyKvp.Key}");
                }
            }
        }

        public void OverwriteFrom(ObservableProperty source)
        {
            object newValue;

            foreach (var propertyName in allPropertiesByName.Keys)
            {
                if (!source.TryGetMember(propertyName, out newValue))
                {
                    throw new Exception($"Could not copy from source. Property name: {propertyName}");
                }

                if (!TrySetMember(propertyName, newValue))
                {
                    throw new Exception($"Could not copy to target. Property name: {propertyName}");
                }
            }
        }

        public void ResetValues()
        {
            foreach (var observedProperty in observedProperties.Values)
            {
                observedProperty.ResetValues();
            }

            Initialize();

            foreach (var propertyName in allPropertiesByName.Keys)
            {
                statePropertyChangedEvent?.Invoke(this, new StatePropertyChangedEventArgs
                {
                    PropertyName = propertyName
                });
            }
        }

        public override string ToString()
        {
            return ObservedType.ToString();
        }

        private void Initialize()
        {
            allPropertiesByName = ObservedType.GetProperties().ToDictionary(x => x.Name);
            observedProperties = new Dictionary<string, ObservableProperty>();
            observedArrays = new Dictionary<string, ObservableArray>();
            normalProperties = new Dictionary<string, object>();

            foreach (var propertyKvp in allPropertiesByName)
            {
                string propertyName = propertyKvp.Key;
                PropertyInfo propertyInfo = propertyKvp.Value;

                if (propertyInfo.HasObservableAttribute())
                {
                    if (!propertyInfo.PropertyType.IsInterface)
                    {
                        throw new Exception("Observables must be an interface.");
                    }

                    observedProperties.Add(propertyName, CreateObservableProperty(propertyInfo));
                }
                else if (propertyInfo.HasObservableArrayAttribute())
                {
                    if (!propertyInfo.PropertyType.IsInterface)
                    {
                        throw new Exception("Observables must be an interface.");
                    }

                    if (!IsSupportedArrayType(propertyInfo.PropertyType))
                    {
                        throw new Exception($"Type {propertyInfo.PropertyType} is not supported array type.");
                    }

                    observedArrays.Add(propertyName, CreateObservableArray(propertyInfo));
                }
                else
                {
                    normalProperties.Add(propertyKvp.Key, GetDefault(propertyKvp.Value.PropertyType));
                }
            }
        }

        private bool IsSupportedArrayType(Type type)
        {
            return type.IsGenericType && (
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                || type.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        private ObservableProperty CreateObservableProperty(PropertyInfo property)
        {
            Type valueType = property.PropertyType;

            return new ObservableProperty(valueType, 
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }

        private ObservableArray CreateObservableArray(PropertyInfo property, IEnumerable<object> elements = null)
        {
            Type valueType = property.PropertyType;
            Type observableArrayType = typeof(ObservableArray<>).MakeGenericType(valueType.GetGenericArguments()[0]);

            object[] parameters;
            if (elements != null)
            {
                parameters = new object[] { elements, collectionItemsChangedEvent };
            }
            else
            {
                parameters = new object[] { collectionItemsChangedEvent };
            }

            return (ObservableArray)Activator.CreateInstance(observableArrayType, parameters);
        }

        private object GetDefault(Type t)
        {
            var defaultValues = new Dictionary<Type, object>
            {
                { typeof(int), default(int) },
                { typeof(string), default(string) }
            };

            if (defaultValues.ContainsKey(t))
            {
                return defaultValues[t];
            }
            else if (t.IsClass)
            {
                return null;
            }

            return GetType()
                .GetMethod(nameof(GetDefaultGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(t)
                .Invoke(this, null);
        }

        private T GetDefaultGeneric<T>()
        {
            return default;
        }
    }
}
