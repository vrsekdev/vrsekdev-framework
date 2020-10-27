using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Attributes;
using Havit.Blazor.Mobx.Abstractions.Events;
using Havit.Blazor.Mobx.Abstractions.Utils;
using Havit.Blazor.Mobx.Observables.Default.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Observables.Default
{
    internal class ObservableProperty : IObservableProperty
    {
        private EventHandler<ObservablePropertyStateChangedEventArgs> statePropertyChangedEvent;
        private EventHandler<ObservableCollectionItemsChangedEventArgs> collectionItemsChangedEvent;

        private Dictionary<string, PropertyInfo> allPropertiesByName;

        private Dictionary<string, ObservableProperty> observedProperties;
        private Dictionary<string, ObservableCollection> observedCollections;
        private Dictionary<string, LazyDefault<object>> normalProperties;
        private bool disposed;

        public Type ObservedType { get; }

        internal ObservableProperty(
            Type interfaceType,
            EventHandler<ObservablePropertyStateChangedEventArgs> statePropertyChangedEvent,
            EventHandler<ObservableCollectionItemsChangedEventArgs> collectionItemsChangedEvent)
        {
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

            if (observedCollections.ContainsKey(name))
            {
                result = observedCollections[name];

                return true;
            }

            if (normalProperties.ContainsKey(name))
            {
                result = normalProperties[name].Value;

                return true;
            }

            result = null;
            return false;
        }

        public bool TrySetDefaultValue(string name, object value)
        {
            if (normalProperties.TryGetValue(name, out LazyDefault<object> currentValue))
            {
                if (currentValue.IsValueSet)
                {
                    return true;
                }
            }

            return TrySetMemberInternal(name, value, false);
        }

        public bool TrySetMember(string name, object value)
        {
            return TrySetMemberInternal(name, value, true);
        }

        private bool TrySetMemberInternal(string name, object value, bool notify)
        {
            if (!allPropertiesByName.ContainsKey(name))
            {
                return false;
            }

            if (observedProperties.ContainsKey(name))
            {
                DoSetObservedProperty(name, value, notify);

                return true;
            }

            if (observedCollections.ContainsKey(name))
            {
                DoSetObservedCollection(name, value, notify);

                return true;
            }

            if (normalProperties.ContainsKey(name))
            {
                DoSetNormalProperty(name, value, notify);

                return true;
            }

            throw new Exception();
        }

        private void DoSetObservedProperty(string name, object value, bool notify)
        {
            observedProperties[name].OverwriteFrom(value, notify);

            if (notify)
            {
                statePropertyChangedEvent?.Invoke(this, new ObservablePropertyStateChangedEventArgs
                {
                    ObservableProperty = this,
                    PropertyInfo = allPropertiesByName[name]
                });
            }
        }

        private void DoSetObservedCollection(string name, object value, bool notify)
        {
            var oldArray = observedCollections[name];
            if (oldArray == value)
            {
                // Do nothing when collection is identical
                return;
            }
            int oldArrayCount = oldArray.CountElements;

            if (value != null)
            {
                if (!(value is IObservableCollection observableArray))
                {
                    throw new Exception("Unsupported type of array.");
                }

                // TODO: Check logic
                var items = ((IEnumerable<object>)oldArray).FullOuterJoin((IEnumerable<object>)observableArray, (oldItem, newItem) => new
                {
                    NewItem = newItem,
                    OldItem = oldItem
                });

                IEnumerable<object> addedItems = items.Where(x => x.NewItem != null && x.OldItem == null).Select(x => x.NewItem);
                IEnumerable<object> removedItems = items.Where(x => x.OldItem != null && x.NewItem == null).Select(x => x.OldItem);

                oldArray.OverwriteElements(observableArray);

                if (notify)
                {
                    collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
                    {
                        ObservableCollection = oldArray,
                        ItemsAdded = addedItems,
                        ItemsRemoved = removedItems,
                        OldCount = oldArrayCount,
                        NewCount = oldArray.CountElements
                    });
                }
            }
            else
            {
                if (notify)
                {
                    collectionItemsChangedEvent?.Invoke(this, new ObservableCollectionItemsChangedEventArgs
                    {
                        ObservableCollection = oldArray,
                        ItemsAdded = Enumerable.Empty<object>(),
                        ItemsRemoved = (IEnumerable<object>)oldArray,
                        OldCount = oldArrayCount,
                        NewCount = 0
                    });
                }
            }
        }

        private void DoSetNormalProperty(string name, object value, bool notify)
        {
            normalProperties[name].Value = value;

            if (notify)
            {
                statePropertyChangedEvent?.Invoke(this, new ObservablePropertyStateChangedEventArgs
                {
                    ObservableProperty = this,
                    PropertyInfo = allPropertiesByName[name]
                });
            }
        }

        public Dictionary<string, IObservableProperty> GetObservedProperties()
        {
            return observedProperties.ToDictionary(x => x.Key, x => (IObservableProperty)x.Value);
        }

        public Dictionary<string, IObservableCollection> GetObservedCollections()
        {
            return observedCollections.ToDictionary(x => x.Key, x => (IObservableCollection)x.Value);
        }

        public void OverwriteFrom(object source, bool notify)
        {
            if (source == null)
            {
                return;
            }

            if (source is ObservableProperty observableProperty)
            {
                OverwriteFrom(observableProperty, notify);
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
                if (!TrySetMemberInternal(propertyKvp.Key, newValue, notify))
                {
                    throw new Exception($"Could not copy to target. Property name: {propertyKvp.Key}");
                }
            }
        }

        public void OverwriteFrom(IObservableProperty source, bool notify)
        {
            object newValue;
            foreach (var propertyName in allPropertiesByName.Keys)
            {
                if (!source.TryGetMember(propertyName, out newValue))
                {
                    throw new Exception($"Could not copy from source. Property name: {propertyName}");
                }

                if (!TrySetMemberInternal(propertyName, newValue, notify))
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

            foreach (var observedArray in observedCollections.Values)
            {
                observedArray.Reset();
            }

            Initialize();

            foreach (var propertyName in allPropertiesByName.Keys)
            {
                statePropertyChangedEvent?.Invoke(this, new ObservablePropertyStateChangedEventArgs
                {
                    ObservableProperty = this,
                    PropertyInfo = allPropertiesByName[propertyName],
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
                observedCollections = null;
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
            observedCollections = new Dictionary<string, ObservableCollection>();
            normalProperties = new Dictionary<string, LazyDefault<object>>();

            foreach (var propertyKvp in allPropertiesByName)
            {
                string propertyName = propertyKvp.Key;
                PropertyInfo propertyInfo = propertyKvp.Value;

                if (IsSupportedObservableArrayType(propertyInfo.PropertyType))
                {
                    observedCollections.Add(propertyName, CreateEmptyObservableCollection(propertyInfo));
                }
                else if (propertyInfo.HasObservableAttribute())
                {
                    observedProperties.Add(propertyName, CreateObservableProperty(propertyInfo));
                }
                else
                {
                    // TODO: preserve default values?
                    normalProperties.Add(propertyKvp.Key, new LazyDefault<object>(() => GetDefault(propertyKvp.Value.PropertyType)));
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

        private ObservableCollection CreateEmptyObservableCollection(PropertyInfo property)
        {
            Type valueType = property.PropertyType;
            Type observableArrayType = typeof(ObservableCollection<>).MakeGenericType(valueType.GetGenericArguments()[0]);
            bool shouldObserveElement = property.GetCustomAttribute<ObservableAttribute>() != null;

            return (ObservableCollection)Activator.CreateInstance(observableArrayType, shouldObserveElement, statePropertyChangedEvent, collectionItemsChangedEvent);
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
