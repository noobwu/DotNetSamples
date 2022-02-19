using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoobCore.Text
{
    /// <summary>
    /// Creates an instance of a Type from a string value
    /// </summary>
    public static class TypeSerializer
    {
        /// <summary>
        /// Serializes to string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string SerializeToString<T>(T value)
        {
            if (value == null || value is Delegate) return null;
            return JsonSerializer.Serialize(value); ;
        }
    }
}
