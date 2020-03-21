using Havit.Blazor.StateManagement.Mobx.Abstractions.Attributes;
using System.Reflection;

namespace Havit.Blazor.StateManagement.Mobx.Observables.Default.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static bool HasObservableAttribute(this PropertyInfo property)
        {
            return property.GetCustomAttribute<ObservableAttribute>() != null;
        }
    }
}
