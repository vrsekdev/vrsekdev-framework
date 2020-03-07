using Havit.Blazor.StateManagement.Mobx.Extensions;
using Havit.Blazor.StateManagement.Mobx.Models;
using Havit.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObservableProperty : IObservable
    {
        private readonly EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent;
        private readonly EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private Dictionary<string, ObservableProperty> observedProperties;
        private Dictionary<string, ObservableArrayInternal> observedArrays;
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

        internal static ObservableProperty CreateEmptyCopyWithoutCallbacks(ObservableProperty observableProperty)
        {
            var newObservableProperty = new ObservableProperty(
                observableProperty.ObservedType,
                null,
                null);

            return newObservableProperty;
        }

        internal static ObservableProperty CreateWithoutCallbacks(Type observedType)
        {
            var newObservableProperty = new ObservableProperty(
                observedType,
                null,
                null);

            return newObservableProperty;
        }

        internal ObservableProperty(
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
                if (!(value is IObservableArray observableArray))
                {
                    throw new Exception("Unsupported type of array.");
                }

                var oldArray = observedArrays[name];
                var items = oldArray.FullOuterJoin((IEnumerable<object>)observableArray, oldItem => oldItem, newItem => newItem, (oldItem, newItem) => new
                {
                    NewItem = newItem,
                    OldItem = oldItem
                });

                if (!(observableArray is ObservableArrayInternal newArray))
                {
                    newArray = CreateObservableArray(observableArray, true);
                }

                observedArrays[name] = newArray;

                IEnumerable<object> addedItems = items.Where(x => x.NewItem != null && x.OldItem == null).Select(x => x.NewItem);
                IEnumerable<object> removedItems = items.Where(x => x.OldItem != null && x.NewItem == null).Select(x => x.OldItem);

                collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs
                {
                    ItemsAdded = addedItems,
                    ItemsRemoved = removedItems,
                    OldCount = oldArray.Count,
                    NewCount = newArray.Count
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

        public Dictionary<string, ObservableArrayInternal> GetObservedArrays()
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
            observedArrays = new Dictionary<string, ObservableArrayInternal>();
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
                else if (IsSupportedObservableArrayType(propertyInfo.PropertyType))
                {
                    observedArrays.Add(propertyName, CreateEmptyObservableArray(propertyInfo));
                }
                else
                {
                    normalProperties.Add(propertyKvp.Key, GetDefault(propertyKvp.Value.PropertyType));
                }
            }
        }

        private bool IsSupportedObservableArrayType(Type type)
        {
            return type.IsGenericType && 
                type.GetGenericTypeDefinition() == typeof(IObservableArray<>);
        }

        private ObservableProperty CreateObservableProperty(PropertyInfo property)
        {
            Type valueType = property.PropertyType;

            return new ObservableProperty(valueType, 
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }

        private ObservableArrayInternal CreateEmptyObservableArray(PropertyInfo property)
        {
            Type valueType = property.PropertyType;
            Type observableArrayType = typeof(ObservableArrayInternal<>).MakeGenericType(valueType.GetGenericArguments()[0]);

            object[] parameters = new object[] { statePropertyChangedEvent, collectionItemsChangedEvent };

            return (ObservableArrayInternal)Activator.CreateInstance(observableArrayType, BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
        }

        private ObservableArrayInternal CreateObservableArray(IObservableArray observableArray, bool suppressEvent)
        {
            if (observableArray is ObservableArrayInternal observableArrayInternal)
            {
                return observableArrayInternal;
            }

            Type observableArrayType = observableArray.GetType();
            if (!observableArrayType.IsGenericType)
            {
                throw new Exception($"Unexpected implementation of {observableArrayType.Name}");
            }

            Type elementType = observableArrayType.GetGenericArguments()[0];

            return (ObservableArrayInternal)GetType()
                .GetMethod(nameof(CreateObservableArrayGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(elementType)
                .Invoke(this, new object[] { observableArray, suppressEvent });
        }

        private ObservableArrayInternal CreateObservableArrayGeneric<T>(IObservableArray<T> observableArray, bool suppressEvent)
        {
            IEnumerable<T> elements = null;
            if (observableArray is ObservableArrayAdapter<T> adapter)
            {
                elements = adapter;
            }

            ObservableArrayInternal<T> observableArrayInternal = new ObservableArrayInternal<T>(statePropertyChangedEvent, collectionItemsChangedEvent);
            if (elements != null)
            {
                observableArrayInternal.AddRangeInternal(elements, suppressEvent);
            }

            return observableArrayInternal;
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
