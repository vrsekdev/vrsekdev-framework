using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Blazor.Mobx.Observables.Default.Extensions
{
    internal static class EnumerableExtensions
    {
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
