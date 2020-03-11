using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Abstractions.Events;
using Havit.Blazor.StateManagement.Mobx.ObservableProperties.Default.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.ObservableProperties.Default
{
    internal class ObservableProperty : IObservableProperty
    {
        private EventHandler<ObservablePropertyStateChangedEventArgs> statePropertyChangedEvent;
        private EventHandler<ObservableCollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private Dictionary<string, ObservableProperty> observedProperties;
        private Dictionary<string, ObservableCollection> observedArrays;
        private Dictionary<string, PropertyInfo> allPropertiesByName;
        private Dictionary<string, object> normalProperties;
        private bool disposed;

        public Type ObservedType { get; }

        internal ObservableProperty(
            Type interfaceType,
            EventHandler<ObservablePropertyStateChangedEventArgs> statePropertyChangedEvent,
            EventHandler<ObservableCollectionItemsChangedEventArgs> collectionItemsChangedEvent)
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

                statePropertyChangedEvent?.Invoke(this, new ObservablePropertyStateChangedEventArgs
                {
                    PropertyInfo = allPropertiesByName[name],
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

                collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
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

                statePropertyChangedEvent?.Invoke(this, new ObservablePropertyStateChangedEventArgs
                {
                    PropertyInfo = allPropertiesByName[name],
                    PropertyName = name
                });

                return true;
            }

            throw new Exception();
        }

        public Dictionary<string, IObservableProperty> GetObservedProperties()
        {
            return observedProperties.ToDictionary(x => x.Key, x => (IObservableProperty)x.Value);
        }

        public Dictionary<string, IObservableCollection> GetObservedCollections()
        {
            return observedArrays.ToDictionary(x => x.Key, x => (IObservableCollection)x.Value);
        }

        public void OverwriteFrom(object source)
        {
            if (source is ObservableProperty observableProperty)
            {
                OverwriteFrom(observableProperty);
                observableProperty.Dispose();
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
                statePropertyChangedEvent?.Invoke(this, new ObservablePropertyStateChangedEventArgs
                {
                    PropertyInfo = allPropertiesByName[propertyName],
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

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;

                statePropertyChangedEvent = null;
                collectionItemsChangedEvent = null;
                observedProperties = null;
                observedArrays = null;
                allPropertiesByName = null;
                normalProperties = null;
            }
#if DEBUG
            else
            {
                throw new Exception("Already disposed.");
            }
#endif
        }

        public override string ToString()
        {
            return ObservedType.ToString();
        }

        private void Initialize()
        {
            allPropertiesByName = ObservedType.GetProperties().ToDictionary(x => x.Name);
            observedProperties = new Dictionary<string, ObservableProperty>();
            observedArrays = new Dictionary<string, ObservableCollection>();
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

        private ObservableProperty CreateObservableProperty(PropertyInfo property)
        {
            Type valueType = property.PropertyType;

            return new ObservableProperty(valueType, 
                statePropertyChangedEvent,
                collectionItemsChangedEvent);
        }

        private ObservableCollection CreateEmptyObservableArray(PropertyInfo property)
        {
            Type valueType = property.PropertyType;
            Type observableArrayType = typeof(ObservableCollection<>).MakeGenericType(valueType.GetGenericArguments()[0]);

            return (ObservableCollection)Activator.CreateInstance(observableArrayType, statePropertyChangedEvent, collectionItemsChangedEvent);
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
