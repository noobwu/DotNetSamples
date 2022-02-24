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

        /// <summary>
        /// Converts to filesizedisplay.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns>System.String.</returns>
        public static string ToFileSizeDisplay(this int i)
        {
            return ToFileSizeDisplay((long)i, 2);
        }

        /// <summary>
        /// Converts to filesizedisplay.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="decimals">The decimals.</param>
        /// <returns>System.String.</returns>
        public static string ToFileSizeDisplay(this int i, int decimals)
        {
            return ToFileSizeDisplay((long)i, decimals);
        }

        /// <summary>
        /// Converts to filesizedisplay.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns>System.String.</returns>
        public static string ToFileSizeDisplay(this long i)
        {
            return ToFileSizeDisplay(i, 2);
        }

        /// <summary>
        /// Converts to filesizedisplay.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="decimals">The decimals.</param>
        /// <returns>System.String.</returns>
        public static string ToFileSizeDisplay(this long i, int decimals)
        {
            if (i < 1024 * 1024 * 1024) // 1 GB
            {
                string value = Math.Round(i / 1024m / 1024m, decimals).ToString("N" + decimals);
                if (decimals > 0 && value.EndsWith(new string('0', decimals)))
                {
                    value = value.Substring(0, value.Length - decimals - 1);
                }
                return string.Concat(value, " MB");
            }
            else
            {
                string value = Math.Round(i / 1024m / 1024m / 1024m, decimals).ToString("N" + decimals);
                if (decimals > 0 && value.EndsWith(new string('0', decimals)))
                {
                    value = value.Substring(0, value.Length - decimals - 1);
                }

                return string.Concat(value, " GB");
            }
        }

        /// <summary>
        /// Converts to ordinal.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <returns>System.String.</returns>
        public static string ToOrdinal(this int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num.ToString("#,###0") + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num.ToString("#,###0") + "st";
                case 2:
                    return num.ToString("#,###0") + "nd";
                case 3:
                    return num.ToString("#,###0") + "rd";
                default:
                    return num.ToString("#,###0") + "th";
            }
        }

        // Load all suffixes in an array          
        /// <summary>
        /// The suffixes
        /// </summary>
        static readonly string[] suffixes ={ "Bytes", "KB", "MB", "GB", "TB", "PB" };
        /// <summary>
        /// Formats the size.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }

}