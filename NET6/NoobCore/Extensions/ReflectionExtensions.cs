using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public delegate EmptyCtorDelegate EmptyCtorFactoryDelegate(Type type);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate object EmptyCtorDelegate();
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the public properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetTypesPublicProperties();

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetTypesPublicProperties()
                .Where(t => t.GetIndexParameters().Length == 0) // ignore indexed properties
                .ToArray();
        }

        /// <summary>
        /// Determines whether [is nullable type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        /// <summary>
        /// Gets the type code.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static TypeCode GetTypeCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }

        /// <summary>
        /// Determines whether [is instance of] [the specified this or base type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="thisOrBaseType">Type of the this or base.</param>
        /// <returns>
        ///   <c>true</c> if [is instance of] [the specified this or base type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInstanceOf(this Type type, Type thisOrBaseType)
        {
            while (type != null)
            {
                if (type == thisOrBaseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }
}
