using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Havit.Blazor.StateManagement.Mobx.Abstractions;

namespace Havit.Blazor.StateManagement.Mobx.Extensions
{
    public static class EnumerableExtensions
    {
        public static IObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollectionAdapter<T>(source);
        }
	}
}
