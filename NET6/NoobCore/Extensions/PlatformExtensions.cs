//Url: https://github.com/ServiceStack/ServiceStack.Text/blob/master/src/ServiceStack.Text/PlatformExtensions.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    public static class PlatformExtensions
    {
        /// <summary>
        /// Gets the static method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo GetStaticMethod(this Type type, string methodName) =>
           type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        /// <summary>
        /// Gets the instance method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo GetInstanceMethod(this Type type, string methodName) =>
            type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        /// <summary>
        /// Makes the delegate.
        /// </summary>
        /// <param name="mi">The mi.</param>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="throwOnBindFailure">if set to <c>true</c> [throw on bind failure].</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Delegate MakeDelegate(this MethodInfo mi, Type delegateType, bool throwOnBindFailure = true) =>
         Delegate.CreateDelegate(delegateType, mi, throwOnBindFailure);

        /// <summary>
        /// Gets the types public properties.
        /// </summary>
        /// <param name="subType">Type of the sub.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static PropertyInfo[] GetTypesPublicProperties(this Type subType)
        {
            return subType.GetProperties(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.Instance);
        }
        //Should only register Runtime Attributes on StartUp, So using non-ThreadSafe Dictionary is OK        
        /// <summary>
        /// The property attributes map
        /// </summary>
        static Dictionary<string, List<Attribute>> propertyAttributesMap = new();

        /// <summary>
        /// The type attributes map
        /// </summary>
        static Dictionary<Type, List<Attribute>> typeAttributesMap = new();

        /// <summary>
        /// Clears the runtime attributes.
        /// </summary>
        public static void ClearRuntimeAttributes()
        {
            propertyAttributesMap = new Dictionary<string, List<Attribute>>();
            typeAttributesMap = new Dictionary<Type, List<Attribute>>();
        }
        /// <summary>
        /// Uniques the key.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Property '{0}' has no DeclaringType".Fmt(pi.Name)</exception>
        internal static string UniqueKey(this PropertyInfo pi)
        {
            if (pi.DeclaringType == null)
                throw new ArgumentException($"Property '{pi.Name}' has no DeclaringType");

            return pi.DeclaringType.Namespace + "." + pi.DeclaringType.Name + "." + pi.Name;
        }
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        public static List<TAttr> GetAttributes<TAttr>(this PropertyInfo propertyInfo)
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out var propertyAttrs)
                ? new List<TAttr>()
                : propertyAttrs.OfType<TAttr>().ToList();
        }
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        public static List<Attribute> GetAttributes(this PropertyInfo propertyInfo)
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out var propertyAttrs)
                ? new List<Attribute>()
                : propertyAttrs.ToList();
        }
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns></returns>
        public static List<Attribute> GetAttributes(this PropertyInfo propertyInfo, Type attrType)
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out var propertyAttrs)
                ? new List<Attribute>()
                : propertyAttrs.Where(x => attrType.IsInstanceOf(x.GetType())).ToList();
        }
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        public static object[] AllAttributes(this PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(true);
            var runtimeAttrs = propertyInfo.GetAttributes();
            if (runtimeAttrs.Count == 0)
                return attrs;

            runtimeAttrs.AddRange(attrs.Cast<Attribute>());
            return runtimeAttrs.Cast<object>().ToArray();
        }
        /// <summary>
        /// Alls the attributes lazy.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        public static IEnumerable<object> AllAttributesLazy(this PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(true);
            var runtimeAttrs = propertyInfo.GetAttributes();
            foreach (var attr in runtimeAttrs)
            {
                yield return attr;
            }
            foreach (var attr in attrs)
            {
                yield return attr;
            }
        }
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns></returns>
        public static object[] AllAttributes(this PropertyInfo propertyInfo, Type attrType)
        {
            var attrs = propertyInfo.GetCustomAttributes(attrType, true);
            var runtimeAttrs = propertyInfo.GetAttributes(attrType);
            if (runtimeAttrs.Count == 0)
                return attrs;

            runtimeAttrs.AddRange(attrs.Cast<Attribute>());
            return runtimeAttrs.Cast<object>().ToArray();
        }
        /// <summary>
        /// Alls the attributes lazy.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns></returns>
        public static IEnumerable<object> AllAttributesLazy(this PropertyInfo propertyInfo, Type attrType)
        {
            foreach (var attr in propertyInfo.GetAttributes(attrType))
            {
                yield return attr;
            }
            foreach (var attr in propertyInfo.GetCustomAttributes(attrType, true))
            {
                yield return attr;
            }
        }
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="paramInfo">The parameter information.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this ParameterInfo paramInfo) => paramInfo.GetCustomAttributes(true);
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this FieldInfo fieldInfo) => fieldInfo.GetCustomAttributes(true);
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="memberInfo">The member information.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this MemberInfo memberInfo) => memberInfo.GetCustomAttributes(true);
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="paramInfo">The parameter information.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this ParameterInfo paramInfo, Type attrType) => paramInfo.GetCustomAttributes(attrType, true);

        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="memberInfo">The member information.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this MemberInfo memberInfo, Type attrType)
        {
            var prop = memberInfo as PropertyInfo;
            return prop != null
                ? prop.AllAttributes(attrType)
                : memberInfo.GetCustomAttributes(attrType, true);
        }

        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this FieldInfo fieldInfo, Type attrType) => fieldInfo.GetCustomAttributes(attrType, true);
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this Type type) => type.GetCustomAttributes(true).Union(type.GetRuntimeAttributes()).ToArray();
        /// <summary>
        /// Alls the attributes lazy.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
       [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<object> AllAttributesLazy(this Type type)
        {
            foreach (var attr in type.GetRuntimeAttributes())
                yield return attr;
            foreach (var attr in type.GetCustomAttributes(true))
                yield return attr;
        }
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this Type type, Type attrType) =>
            type.GetCustomAttributes(attrType, true).Union(type.GetRuntimeAttributes(attrType)).ToArray();
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this Assembly assembly) => assembly.GetCustomAttributes(true).ToArray();
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="pi">The pi.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this ParameterInfo pi) => pi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="mi">The mi.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this MemberInfo mi) => mi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="fi">The fi.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this FieldInfo fi) => fi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="pi">The pi.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this PropertyInfo pi) => pi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        /// <summary>
        /// Alls the attributes lazy.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="pi">The pi.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TAttr> AllAttributesLazy<TAttr>(this PropertyInfo pi) => pi.AllAttributesLazy(typeof(TAttr)).Cast<TAttr>();
        /// <summary>
        /// Gets the runtime attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<T> GetRuntimeAttributes<T>(this Type type) => typeAttributesMap.TryGetValue(type, out var attrs)
            ? attrs.OfType<T>()
            : new List<T>();

        /// <summary>
        /// Gets the runtime attributes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<Attribute> GetRuntimeAttributes(this Type type, Type attrType = null) => typeAttributesMap.TryGetValue(type, out var attrs)
            ? attrs.Where(x => attrType == null || attrType.IsInstanceOf(x.GetType()))
            : new List<Attribute>();

        /// <summary>
        /// Alls the attributes.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this Type type)
        {
            return type.GetCustomAttributes(typeof(TAttr), true)
                .OfType<TAttr>()
                .Union(type.GetRuntimeAttributes<TAttr>())
                .ToArray();
        }
        /// <summary>
        /// Alls the attributes lazy.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TAttr> AllAttributesLazy<TAttr>(this Type type)
        {
            foreach (var attr in type.GetCustomAttributes(typeof(TAttr), true).OfType<TAttr>())
            {
                yield return attr;
            }
            foreach (var attr in type.GetRuntimeAttributes<TAttr>())
            {
                yield return attr;
            }
        }
        /// <summary>
        /// Firsts the attribute.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr FirstAttribute<TAttr>(this Type type) where TAttr : class
        {
            return (TAttr)type.GetCustomAttributes(typeof(TAttr), true)
                       .FirstOrDefault()
                   ?? type.GetRuntimeAttributes<TAttr>().FirstOrDefault();
        }
        /// <summary>
        /// Firsts the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttribute FirstAttribute<TAttribute>(this MemberInfo memberInfo)
        {
            return memberInfo.AllAttributes<TAttribute>().FirstOrDefault();
        }
        /// <summary>
        /// Firsts the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="paramInfo">The parameter information.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttribute FirstAttribute<TAttribute>(this ParameterInfo paramInfo)
        {
            return paramInfo.AllAttributes<TAttribute>().FirstOrDefault();
        }
        /// <summary>
        /// Firsts the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttribute FirstAttribute<TAttribute>(this PropertyInfo propertyInfo) =>
            propertyInfo.AllAttributesLazy<TAttribute>().FirstOrDefault();

        /// <summary>
        /// Gets the public static field.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo GetPublicStaticField(this Type type, string fieldName) =>
           type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
    }
}
