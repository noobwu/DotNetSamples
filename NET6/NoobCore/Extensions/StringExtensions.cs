using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Splits the on first.
        /// </summary>
        /// <param name="strVal">The string value.</param>
        /// <param name="needle">The needle.</param>
        /// <returns></returns>
        public static string[] SplitOnFirst(this string strVal, char needle)
        {
            if (strVal == null) return TypeConstants.EmptyStringArray;
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }
        /// <summary>
        /// Splits the on first.
        /// </summary>
        /// <param name="strVal">The string value.</param>
        /// <param name="needle">The needle.</param>
        /// <returns></returns>
        public static string[] SplitOnFirst(this string strVal, string needle)
        {
            if (strVal == null) return TypeConstants.EmptyStringArray;
            var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + needle.Length) };
        }
        /// <summary>
        /// Splits the on last.
        /// </summary>
        /// <param name="strVal">The string value.</param>
        /// <param name="needle">The needle.</param>
        /// <returns></returns>
        public static string[] SplitOnLast(this string strVal, char needle)
        {
            if (strVal == null) return TypeConstants.EmptyStringArray;
            var pos = strVal.LastIndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }
        /// <summary>
        /// Splits the on last.
        /// </summary>
        /// <param name="strVal">The string value.</param>
        /// <param name="needle">The needle.</param>
        /// <returns></returns>
        public static string[] SplitOnLast(this string strVal, string needle)
        {
            if (strVal == null) return TypeConstants.EmptyStringArray;
            var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + needle.Length) };
        }

        /// <summary>
        /// Determines whether this instance is int.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///   <c>true</c> if the specified text is int; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInt(this string text) => !string.IsNullOrEmpty(text) && int.TryParse(text, out _);
        /// <summary>
        /// Converts the string representation of a number to an integer.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static int ToInt(this string text) => text == null ? default(int) : int.Parse(text);
        /// <summary>
        /// Converts the string representation of a number to an integer.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int ToInt(this string text, int defaultValue) => int.TryParse(text, out var ret) ? ret : defaultValue;
    }
}
