using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class ObservableProperty : IObservableProperty
    {
        private readonly EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent;
        private readonly EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private Dictionary<string, IObservableProperty> observedProperties;
        private Dictionary<string, IObservableCollection> observedArrays;
        private Dictionary<string, PropertyInfo> allPropertiesByName;
        private Dictionary<string, object> normalProperties;

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

        internal ObservableProperty(
            Type interfaceType,
            EventHandler<StatePropertyChangedEventArgs> statePropertyChangedEvent,
            EventHandler<CollectionItemsChangedEventArgs> collectionItemsChangedEvent)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Only interfaces can be observable.");
            }

            ObservedType = interfaceType;
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

            result = null;
            return false;
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
                if (!(value is IObservableCollection observableArray))
                {
                    throw new Exception("Unsupported type of array.");
                }

                var oldArray = observedArrays[name];
                if (oldArray == value)
                {
                    // Do nothing when collection is identical
                    return true;
                }

                // TODO: Check logic
                var items = ((IEnumerable<object>)oldArray).FullOuterJoin((IEnumerable<object>)observableArray, (oldItem, newItem) => new
                {
                    NewItem = newItem,
                    OldItem = oldItem
                });

                IEnumerable<object> addedItems = items.Where(x => x.NewItem != null && x.OldItem == null).Select(x => x.NewItem);
                IEnumerable<object> removedItems = items.Where(x => x.OldItem != null && x.NewItem == null).Select(x => x.OldItem);

                int oldArrayCount = oldArray.CountElements;
                oldArray.OverwriteElements(observableArray);

                collectionItemsChangedEvent?.Invoke(this, new CollectionItemsChangedEventArgs
                {
                    ItemsAdded = addedItems,
                    ItemsRemoved = removedItems,
                    OldCount = oldArrayCount,
                    NewCount = oldArray.CountElements
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

        public Dictionary<string, IObservableProperty> GetObservedProperties()
        {
            return observedProperties;
        }

        public Dictionary<string, IObservableCollection> GetObservedCollections()
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

        public void OverwriteFrom(IObservableProperty source)
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

            foreach (var observedArray in observedArrays.Values)
            {
                observedArray.Reset();
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

        public IObservableFactory CreateFactory()
        {
            return new ObservableFactory(
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }

        public override string ToString()
        {
            return ObservedType.ToString();
        }

        private void Initialize()
        {
            allPropertiesByName = ObservedType.GetProperties().ToDictionary(x => x.Name);
            observedProperties = new Dictionary<string, IObservableProperty>();
            observedArrays = new Dictionary<string, IObservableCollection>();
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
                type.GetGenericTypeDefinition() == typeof(IObservableCollection<>);
        }

        private IObservableProperty CreateObservableProperty(PropertyInfo property)
        {
            Type valueType = property.PropertyType;

            return new ObservableProperty(valueType, 
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }

        private IObservableCollection CreateEmptyObservableArray(PropertyInfo property)
        {
            Type valueType = property.PropertyType;
            Type observableArrayType = typeof(ObservableCollection<>).MakeGenericType(valueType.GetGenericArguments()[0]);

            return (IObservableCollection)Activator.CreateInstance(observableArrayType, statePropertyChangedEvent, collectionItemsChangedEvent);
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
