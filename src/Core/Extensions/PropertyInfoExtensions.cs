using Havit.Blazor.StateManagement.Mobx.Attributes;
using Microsoft.AspNetCore.Components;
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

        public static bool HasObservableArrayElementAttribute(this Type type)
        {
            return type.GetCustomAttribute<ObservableArrayElementAttribute>() != null;
        }

        public static bool IsParameterProperty(this PropertyInfo property)
        {
            return property.GetCustomAttribute<ParameterAttribute>() != null;
        }
    }
}
