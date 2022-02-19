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
    /// <param name="instance">The instance.</param>
    /// <returns></returns>
    public delegate object GetMemberDelegate(object instance);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance">The instance.</param>
    /// <returns></returns>
    public delegate object GetMemberDelegate<T>(T instance);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="value">The value.</param>
    public delegate void SetMemberDelegate(object instance, object value);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance">The instance.</param>
    /// <param name="value">The value.</param>
    public delegate void SetMemberDelegate<T>(T instance, object value);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="propertyValue">The property value.</param>
    public delegate void SetMemberRefDelegate(ref object instance, object propertyValue);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance">The instance.</param>
    /// <param name="value">The value.</param>
    public delegate void SetMemberRefDelegate<T>(ref T instance, object value);

    public static class AutoMappingUtils
    {
        /// <summary>
        /// The default value types
        /// </summary>
        private static Dictionary<Type, object> DefaultValueTypes = new Dictionary<Type, object>();
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type type)
        {
            if (!type.IsValueType)
                return null;

            if (DefaultValueTypes.TryGetValue(type, out var defaultValue))
                return defaultValue;

            defaultValue = Activator.CreateInstance(type);

            Dictionary<Type, object> snapshot, newCache;
            do
            {
                snapshot = DefaultValueTypes;
                newCache = new Dictionary<Type, object>(DefaultValueTypes) { [type] = defaultValue };

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref DefaultValueTypes, newCache, snapshot), snapshot));

            return defaultValue;
        }
        public static bool IsDefaultValue(object value, Type valueType) => value == null
    || (valueType.IsValueType && value.Equals(valueType.GetDefaultValue()));
    }
}
