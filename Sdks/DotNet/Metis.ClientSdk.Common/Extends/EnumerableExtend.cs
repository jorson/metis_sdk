using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System.Linq
{
    internal static class EnumerableExtend
    {
        public static void ForEachG<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }

        public static void ForG<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            int index = 0;
            foreach (T item in items)
            {
                action(item, index);
                index++;
            }
        }

        public static IList<TOutput> ForG<T, TOutput>(this IEnumerable<T> items, Func<T, int, TOutput> func)
        {
            List<TOutput> outputs = new List<TOutput>();
            int index = 0;
            foreach (T item in items)
            {
                outputs.Add(func(item, index));
                index++;
            }
            return outputs;
        }

        /// <summary>
        /// 复制对象集合，仅拷贝集合，对象本身不拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<T> CopyG<T>(this IEnumerable<T> items, int index = 0, int length = 0)
        {
            if (length == 0) length = items.Count();
            var result = new List<T>(length);
            items.ForEachG(o => { result.Add(o); });
            return result;
        }

        public static IEnumerable<Tuple<TSource, TJoin>> TupleJoinG<TSource, TJoin, TKey>(this IEnumerable<TSource> source,
            IEnumerable<TJoin> join,
            Func<TSource, TKey> sourceKeySelector,
            Func<TJoin, TKey> joinKeySelector)
        {
            return source.Join<TSource, TJoin, TKey, Tuple<TSource, TJoin>>(join, sourceKeySelector, joinKeySelector, (sourceItem, joinItem) =>
            {
                return new Tuple<TSource, TJoin>(sourceItem, joinItem);
            });
        }

        public static IEnumerable<TResult> LeftJoinG<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            Dictionary<TKey, TInner> dic = inner.ToDictionary(innerKeySelector);

            return outer.Select(item =>
            {
                TKey key = outerKeySelector(item);
                TInner findItem = dic.TryGetValueG(key);
                return resultSelector(item, findItem);
            });
        }

        public static IEnumerable<TResult> LeftJoinG<TOuter, TInner1, TInner2, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner1> inner1, IEnumerable<TInner2> inner2, Func<TOuter, TKey> outerKeySelector, Func<TInner1, TKey> innerKeySelector1, Func<TInner2, TKey> innerKeySelector2, Func<TOuter, TInner1, TInner2, TResult> resultSelector)
        {
            Dictionary<TKey, TInner1> dic1 = inner1.ToList().Where(o => !IsDefaultG(o)).ToDictionary(innerKeySelector1);
            Dictionary<TKey, TInner2> dic2 = inner2.ToList().Where(o => !IsDefaultG(o)).ToDictionary(innerKeySelector2);

            return outer.Select(item =>
            {
                TKey key = outerKeySelector(item);
                TInner1 findItem1 = dic1.TryGetValueG(key);
                TInner2 findItem2 = dic2.TryGetValueG(key);
                return resultSelector(item, findItem1, findItem2);
            });
        }

        static bool IsDefaultG<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        public static TValue TryGetValueG<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            dic.TryGetValue(key, out value);
            return value;
        }

        public static IEnumerable<string> FormatG<T>(this IEnumerable<T> items, string format)
        {
            return items.Select(o => format.FormatG(o));
        }

        public static IEnumerable<string> FormatG<T, TValue>(this IEnumerable<T> items, string format, Func<T, TValue> valueSelector)
        {
            return items.Select(o => format.FormatG(valueSelector(o)));
        }

        public static IEnumerable<TSource> OrderByG<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<TKey> sort)
        {
            return source.Where(o => !IsDefaultG(o)).OrderBy(keySelector, new QueueComparer<TKey>(sort));
        }

        public static HashSet<TKey> ToHashSetG<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return new HashSet<TKey>(source.Select(o => keySelector(o)));
        }

        public static string JoinWithG<TSource>(this IEnumerable<TSource> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static string JoinWithG(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }
    }

    internal class QueueComparer<TKey> : IComparer<TKey>
    {
        Dictionary<TKey, int> dic;

        public QueueComparer(IEnumerable<TKey> keys)
        {
            dic = new Dictionary<TKey, int>();
            int index = 0;
            foreach (TKey key in keys)
            {
                dic[key] = index++;
            }
        }

        public int Compare(TKey x, TKey y)
        {
            return dic[x].CompareTo(dic[y]);
        }
    }
}
