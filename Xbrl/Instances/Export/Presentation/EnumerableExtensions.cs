using System;
using System.Collections.Generic;
using System.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var list = source.ToList();
            return list.SelectMany(c => (new [] {c}).Concat(selector(c).Flatten(selector)));
        }
    }
}