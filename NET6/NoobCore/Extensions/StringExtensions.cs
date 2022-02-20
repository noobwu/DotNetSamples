using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Converts to json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj);
        }
        /// <summary>
        /// Froms the UTF8 bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static string FromUtf8Bytes(this byte[] bytes)
        {
            return bytes == null ? null
                : bytes.Length > 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF
                    ? Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3)
                    : Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// Converts to utf8bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
        /// <summary>
        /// Converts to utf8bytes.
        /// </summary>
        /// <param name="intVal">The int value.</param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this int intVal)
        {
            return FastToUtf8Bytes(intVal.ToString());
        }
        /// <summary>
        /// Converts to utf8bytes.
        /// </summary>
        /// <param name="longVal">The long value.</param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this long longVal)
        {
            return FastToUtf8Bytes(longVal.ToString());
        }
        /// <summary>
        /// Converts to utf8bytes.
        /// </summary>
        /// <param name="ulongVal">The ulong value.</param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this ulong ulongVal)
        {
            return FastToUtf8Bytes(ulongVal.ToString());
        }
        /// <summary>
        /// Converts to utf8bytes.
        /// </summary>
        /// <param name="doubleVal">The double value.</param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this double doubleVal)
        {
            var doubleStr = doubleVal.ToString(CultureInfo.InvariantCulture.NumberFormat);

            if (doubleStr.IndexOf('E') != -1 || doubleStr.IndexOf('e') != -1)
                doubleStr = Text.Support.DoubleConverter.ToExactString(doubleVal);

            return FastToUtf8Bytes(doubleStr);
        }

        /// <summary>
        /// Skip the encoding process for 'safe strings' 
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns></returns>
        private static byte[] FastToUtf8Bytes(string strVal)
        {
            var bytes = new byte[strVal.Length];
            for (var i = 0; i < strVal.Length; i++)
                bytes[i] = (byte)strVal[i];

            return bytes;
        }
        /// <summary>
        /// Withouts the bom.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string WithoutBom(this string value)
        {
            return value.Length > 0 && value[0] == 65279
                ? value.Substring(1)
                : value;
        }

        /// <summary>
        /// Lefts the part.
        /// </summary>
        /// <param name="strVal">The string value.</param>
        /// <param name="needle">The needle.</param>
        /// <returns></returns>
        public static string LeftPart(this string strVal, char needle)
        {
            if (strVal == null) return null;
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? strVal
                : strVal.Substring(0, pos);
        }
        /// <summary>
        /// Lefts the part.
        /// </summary>
        /// <param name="strVal">The string value.</param>
        /// <param name="needle">The needle.</param>
        /// <returns></returns>
        public static string LeftPart(this string strVal, string needle)
        {
            if (strVal == null) return null;
            var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? strVal
                : strVal.Substring(0, pos);
        }

        /// <summary>
        /// The split camel case regex
        /// </summary>
        private static readonly Regex SplitCamelCaseRegex = new("([A-Z]|[0-9]+)", RegexOptions.Compiled);
        /// <summary>
        /// Splits the camel case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string SplitCamelCase(this string value)
        {
            return SplitCamelCaseRegex.Replace(value, " $1").TrimStart();
        }
        /// <summary>
        /// Converts to invariantupper.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToInvariantUpper(this char value)
        {
            return value.ToString().ToUpperInvariant();
        }
        /// <summary>
        /// Converts to english.
        /// </summary>
        /// <param name="camelCase">The camel case.</param>
        /// <returns></returns>
        public static string ToEnglish(this string camelCase)
        {
            var ucWords = camelCase.SplitCamelCase().ToLower();
            return ucWords[0].ToInvariantUpper() + ucWords.Substring(1);
        }
        /// <summary>
        /// Joins the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static string Join(this List<string> items){
            return string.Join(",", items.ToArray());
        }

        /// <summary>
        /// Joins the specified delimeter.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <returns></returns>
        public static string Join(this List<string> items, string delimeter)
        {
            return string.Join(delimeter, items.ToArray());
        }
        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        /// <summary>
        /// Determines whether [is null or empty].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Equalses the ignore case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string value, string other)
        {
            return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Froms the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Converts to rot13.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToRot13(this string value)
        {
            var array = value.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                var number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                    number += (number > 'm') ? -13 : 13;

                else if (number >= 'A' && number <= 'Z')
                    number += (number > 'M') ? -13 : 13;

                array[i] = (char)number;
            }
            return new string(array);
        }
    }
}
