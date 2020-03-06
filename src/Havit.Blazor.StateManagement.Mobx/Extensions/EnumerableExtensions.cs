using Havit.Blazor.StateManagement.Mobx.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Extensions
{
    public static class EnumerableExtensions
    {
        public static IObservableArray<T> ToObservableArray<T>(this IEnumerable<T> source)
        {
            return new ObservableArrayAdapter<T>(source);
        }
    }
}
