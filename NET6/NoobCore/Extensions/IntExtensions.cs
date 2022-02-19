// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Timeses the specified times.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <returns></returns>
        public static IEnumerable<int> Times(this int times)
        {
            for (var i = 0; i < times; i++)
            {
                yield return i;
            }
        }
        /// <summary>
        /// Timeses the specified action function.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        public static void Times(this int times, Action<int> actionFn)
        {
            for (var i = 0; i < times; i++)
            {
                actionFn(i);
            }
        }

        /// <summary>
        /// Timeses the specified action function.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        public static void Times(this int times, Action actionFn)
        {
            for (var i = 0; i < times; i++)
            {
                actionFn();
            }
        }
        /// <summary>
        /// Timeses the specified action function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        /// <returns></returns>
        public static List<T> Times<T>(this int times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (var i = 0; i < times; i++)
            {
                list.Add(actionFn());
            }
            return list;
        }

        /// <summary>
        /// Timeses the specified action function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        /// <returns></returns>
        public static List<T> Times<T>(this int times, Func<int, T> actionFn)
        {
            var list = new List<T>();
            for (var i = 0; i < times; i++)
            {
                list.Add(actionFn(i));
            }
            return list;
        }

        /// <summary>
        /// Timeses the asynchronous.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        /// <param name="token">The token.</param>
        public static async Task TimesAsync(this int times, Func<int, Task> actionFn, CancellationToken token = default)
        {
            for (var i = 0; i < times; i++)
            {
                token.ThrowIfCancellationRequested();
                await actionFn(i);
            }
        }
        /// <summary>
        /// Timeses the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public static async Task<List<T>> TimesAsync<T>(this int times, Func<int, Task<T>> actionFn, CancellationToken token = default)
        {
            var list = new List<T>();
            for (var i = 0; i < times; i++)
            {
                token.ThrowIfCancellationRequested();
                list.Add(await actionFn(i));
            }
            return list;
        }

        /// <summary>
        /// Timeses the asynchronous.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        /// <returns></returns>
        public static List<IAsyncResult> TimesAsync(this int times, Action<int> actionFn)
        {
            var asyncResults = new List<IAsyncResult>(times);
            for (var i = 0; i < times; i++)
            {
                asyncResults.Add(Task.Run(() => actionFn(i)));
            }
            return asyncResults;
        }
        /// <summary>
        /// Timeses the asynchronous.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        /// <returns></returns>
        public static List<IAsyncResult> TimesAsync(this int times, Action actionFn)
        {
            var asyncResults = new List<IAsyncResult>(times);
            for (var i = 0; i < times; i++)
            {
                asyncResults.Add(Task.Run(actionFn));
            }
            return asyncResults;
        }
    }
}