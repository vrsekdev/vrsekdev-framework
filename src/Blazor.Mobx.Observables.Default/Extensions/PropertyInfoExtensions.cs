using Havit.Blazor.Mobx.Abstractions.Attributes;
using System.Reflection;

namespace Havit.Blazor.Mobx.Observables.Default.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static bool HasObservableAttribute(this PropertyInfo property)
        {
            return property.GetCustomAttribute<ObservableAttribute>() != null;
        }
    }
}
