using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace UHub.CoreLib.Extensions
{
    /// <summary>
    /// List extension methods
    /// </summary>
    public static class ListExtensions
    {
        private static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static Random Local;
            public static Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }

        /// <summary>
        /// Randomize order of objects in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Randomize<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Order list by direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static IOrderedEnumerable<TSource> orderByDirectionCore<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection dir = SortDirection.Ascending)
        {
            if (source.Count() > 0)
            {
                if (dir == SortDirection.Ascending)
                {
                    return source.OrderBy(keySelector);
                }
                else
                {
                    return source.OrderByDescending(keySelector);
                }
            }
            else
                return (IOrderedEnumerable<TSource>)Enumerable.Empty<TSource>();
        }

        /// <summary>
        /// Order list  with variable direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="useAsc"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool useAsc = true)
        {
            var dir = useAsc ? SortDirection.Ascending : SortDirection.Descending;
            return orderByDirectionCore(source, keySelector, dir);
        }

        /// <summary>
        /// Order list with variable direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection dir = SortDirection.Ascending)
        {
            return orderByDirectionCore(source, keySelector, dir);
        }

        /// <summary>
        /// Order list with variable direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="useAsc"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource>(this IEnumerable<TSource> source, string sortProperty, bool useAsc = true)
        {
            var dir = useAsc ? SortDirection.Ascending : SortDirection.Descending;

            Func<TSource, object> keySelector = (itm) => typeof(TSource).GetProperty(sortProperty).GetValue(itm, null);

            return orderByDirectionCore(source, keySelector, dir);
        }

        /// <summary>
        /// Order list with variable direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource>(this IEnumerable<TSource> source, string sortProperty, SortDirection dir = SortDirection.Ascending)
        {
            Func<TSource, object> keySelector = (itm) => typeof(TSource).GetProperty(sortProperty).GetValue(itm, null);

            return orderByDirectionCore(source, keySelector, dir);
        }


    }
}
