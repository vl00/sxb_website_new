using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.WenDa.API.Utils
{
    internal static partial class CollectionExtension
    {
        public static List<T> AsList<T>(this IEnumerable<T> collection)
        {
            return collection is List<T> ls ? ls : collection?.ToList();
        }

        public static T[] AsArray<T>(this IEnumerable<T> collection)
        {
            return collection is T[] arr ? arr : collection?.ToArray();
        }

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    var seenKeys = new HashSet<TKey>();
        //    foreach (var element in source)
        //    {
        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}

        public static bool TryGetOne<T>(this IEnumerable<T> enumerable, out T item, Func<T, bool> condition = null)
        {
            item = default;
            foreach (var item0 in enumerable)
            {
                if (condition == null || condition(item0))
                {
                    item = item0;
                    return true;
                }
            }
            return false;
        }

        /// <summary>可能无序的items按ids的顺序来排序并返回</summary>
        internal static IEnumerable<Titems> ItemsOrderBy<Titems, Tid>(this IEnumerable<Titems> items, IEnumerable<Tid> ids, Func<Titems, Tid> func)
            where Tid : IEquatable<Tid>
        {
            foreach (var id in ids)
            {
                var arr = items.Where(_ => id.Equals(func(_)));
                foreach (var a in arr)
                    yield return a;
            }
            if (items.Count() > ids.Count())
            {
                foreach (var a in items.Where(x => !ids.Any(_ => _.Equals(func(x)))))
                    yield return a;
            }
        }

        internal static T ItemsEmptyToNull<T>(this T items) where T : IEnumerable
        {
            var gr = items?.GetEnumerator();
            return gr?.MoveNext() == true ? items : default;
        }
    }
}