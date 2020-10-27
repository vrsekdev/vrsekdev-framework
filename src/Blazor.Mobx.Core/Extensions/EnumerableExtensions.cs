using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using VrsekDev.Blazor.Mobx.Abstractions;

namespace VrsekDev.Blazor.Mobx.Extensions
{
    public static class EnumerableExtensions
    {
        public static IObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollectionAdapter<T>(source);
        }
	}
}
