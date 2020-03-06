using Havit.Blazor.StateManagement.Mobx.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool HasObservableAttribute(this PropertyInfo property)
        {
            return property.GetCustomAttribute<ObservableAttribute>() != null;
        }

        public static bool HasObservableArrayAttribute(this PropertyInfo property)
        {
            return property.GetCustomAttribute<ObservableArrayAttribute>() != null;
        }

        public static bool HasObservableArrayElementAttribute(this Type type)
        {
            return type.GetCustomAttribute<ObservableArrayElementAttribute>() != null;
        }
    }
}
