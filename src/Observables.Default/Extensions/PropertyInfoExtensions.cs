using Havit.Blazor.StateManagement.Mobx.Observables.Default.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Observables.Default.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static bool HasObservableAttribute(this PropertyInfo property)
        {
            return property.GetCustomAttribute<ObservableAttribute>() != null;
        }

        public static bool HasObservableArrayElementAttribute(this Type type)
        {
            return type.GetCustomAttribute<ObservableArrayElementAttribute>() != null;
        }
    }
}
