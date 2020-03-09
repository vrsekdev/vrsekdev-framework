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

		internal static IEnumerable<TResult> FullOuterJoin<TResult>(this IEnumerable<object> leftSource,
										 IEnumerable<object> rightSource,
										 Func<object, object, TResult> resultSelector)
		{
			var leftLookup = leftSource.ToLookup(x => x);
			var rightLookup = rightSource.ToLookup(x => x);

			var keys = new HashSet<object>(leftLookup.Select(p => p.Key));
			keys.UnionWith(rightLookup.Select(p => p.Key));

			IEnumerable<TResult> result = from key in keys
										  from xLeft in leftLookup[key].DefaultIfEmpty()
										  from xRight in rightLookup[key].DefaultIfEmpty()
										  select resultSelector(xLeft, xRight);

			return result.ToList();
		}
	}
}
