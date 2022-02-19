using System;
using System.Text;
//Url：https://github.com/ServiceStack/ServiceStack.Text/blob/master/src/ServiceStack.Text/StringBuilderCache.cs
namespace NoobCore.Text
{
    /// <summary>
    /// Reusable StringBuilder ThreadStatic Cache
    /// </summary>
    public static class StringBuilderCache
    {
        /// <summary>
        /// The cache
        /// </summary>
        [ThreadStatic]
        static StringBuilder cache;

        /// <summary>
        /// Allocates this instance.
        /// </summary>
        /// <returns></returns>
        public static StringBuilder Allocate()
        {
            var ret = cache;
            if (ret == null)
                return new StringBuilder();

            ret.Length = 0;
            cache = null;  //don't re-issue cached instance until it's freed
            return ret;
        }

        /// <summary>
        /// Frees the specified sb.
        /// </summary>
        /// <param name="sb">The sb.</param>
        public static void Free(StringBuilder sb)
        {
            cache = sb;
        }
        /// <summary>
        /// Returns the and free.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <returns></returns>
        public static string ReturnAndFree(StringBuilder sb)
        {
            var ret = sb.ToString();
            cache = sb;
            return ret;
        }
    }

    /// <summary>
    /// Alternative Reusable StringBuilder ThreadStatic Cache
    /// </summary>
    public static class StringBuilderCacheAlt
    {
        /// <summary>
        /// The cache
        /// </summary>
        [ThreadStatic]
        static StringBuilder cache;

        /// <summary>
        /// Allocates this instance.
        /// </summary>
        /// <returns></returns>
        public static StringBuilder Allocate()
        {
            var ret = cache;
            if (ret == null)
                return new StringBuilder();

            ret.Length = 0;
            cache = null;  //don't re-issue cached instance until it's freed
            return ret;
        }

        /// <summary>
        /// Frees the specified sb.
        /// </summary>
        /// <param name="sb">The sb.</param>
        public static void Free(StringBuilder sb)
        {
            cache = sb;
        }

        /// <summary>
        /// Returns the and free.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <returns></returns>
        public static string ReturnAndFree(StringBuilder sb)
        {
            var ret = sb.ToString();
            cache = sb;
            return ret;
        }
    }

    /// <summary>
    /// Use separate cache internally to avoid re-allocations and cache misses    
    /// </summary>
    internal static class StringBuilderThreadStatic
    {
        /// <summary>
        /// The cache
        /// </summary>
        [ThreadStatic]
        static StringBuilder cache;

        /// <summary>
        /// Allocates this instance.
        /// </summary>
        /// <returns></returns>
        public static StringBuilder Allocate()
        {
            var ret = cache;
            if (ret == null)
                return new StringBuilder();

            ret.Length = 0;
            cache = null;  //don't re-issue cached instance until it's freed
            return ret;
        }

        /// <summary>
        /// Frees the specified sb.
        /// </summary>
        /// <param name="sb">The sb.</param>
        public static void Free(StringBuilder sb)
        {
            cache = sb;
        }

        /// <summary>
        /// Returns the and free.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <returns></returns>
        public static string ReturnAndFree(StringBuilder sb)
        {
            var ret = sb.ToString();
            cache = sb;
            return ret;
        }
    }
}