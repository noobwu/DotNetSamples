//Url：https://github.com/ServiceStack/ServiceStack.Text/blob/master/src/ServiceStack.Text/TypeProperties.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NoobCore.Text;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessor"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="publicGetter">The public getter.</param>
        /// <param name="publicSetter">The public setter.</param>
        public PropertyAccessor(
            PropertyInfo propertyInfo,
            GetMemberDelegate publicGetter,
            SetMemberDelegate publicSetter)
        {
            PropertyInfo = propertyInfo;
            PublicGetter = publicGetter;
            PublicSetter = publicSetter;
        }
        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        public PropertyInfo PropertyInfo { get; }
        /// <summary>
        /// Gets the public getter.
        /// </summary>
        /// <value>
        /// The public getter.
        /// </value>
        public GetMemberDelegate PublicGetter { get; }

        /// <summary>
        /// Gets the public setter.
        /// </summary>
        /// <value>
        /// The public setter.
        /// </value>
        public SetMemberDelegate PublicSetter { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="NoobCore.Text.TypeProperties" />
    public class TypeProperties<T> : TypeProperties
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly TypeProperties<T> Instance = new TypeProperties<T>();

        /// <summary>
        /// Initializes the <see cref="TypeProperties{T}"/> class.
        /// </summary>
        static TypeProperties()
        {
            Instance.Type = typeof(T);
            Instance.PublicPropertyInfos = typeof(T).GetPublicProperties();
            foreach (var pi in Instance.PublicPropertyInfos)
            {
                try
                {
                    Instance.PropertyMap[pi.Name] = new PropertyAccessor(
                        pi,
                        ReflectionOptimizer.Instance.CreateGetter(pi),
                        ReflectionOptimizer.Instance.CreateSetter(pi)
                    );
                }
                catch (Exception ex)
                {
                    Tracer.Instance.WriteError(ex);
                }
            }
        }

        /// <summary>
        /// Gets the accessor.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public new static PropertyAccessor GetAccessor(string propertyName)
        {
            return Instance.PropertyMap.TryGetValue(propertyName, out PropertyAccessor info)
                ? info
                : null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public abstract class TypeProperties
    {
        /// <summary>
        /// The cache map
        /// </summary>
        static Dictionary<Type, TypeProperties> CacheMap = new Dictionary<Type, TypeProperties>();

        /// <summary>
        /// The factory type
        /// </summary>
        public static readonly Type FactoryType = typeof(TypeProperties<>);

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static TypeProperties Get(Type type)
        {
            if (CacheMap.TryGetValue(type, out TypeProperties value))
                return value;

            var genericType = FactoryType.MakeGenericType(type);
            var instanceFi = genericType.GetPublicStaticField("Instance");
            var instance = (TypeProperties)instanceFi.GetValue(null);

            Dictionary<Type, TypeProperties> snapshot, newCache;
            do
            {
                snapshot = CacheMap;
                newCache = new Dictionary<Type, TypeProperties>(CacheMap)
                {
                    [type] = instance
                };
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref CacheMap, newCache, snapshot), snapshot));
            return instance;
        }
        /// <summary>
        /// Gets the accessor.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public PropertyAccessor GetAccessor(string propertyName)
        {
            return PropertyMap.TryGetValue(propertyName, out PropertyAccessor info)
                ? info
                : null;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; protected set; }

        /// <summary>
        /// The property map
        /// </summary>
        public readonly Dictionary<string, PropertyAccessor> PropertyMap =
            new Dictionary<string, PropertyAccessor>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the public property infos.
        /// </summary>
        /// <value>
        /// The public property infos.
        /// </value>
        public PropertyInfo[] PublicPropertyInfos { get; protected set; }
        /// <summary>
        /// Gets the public property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public PropertyInfo GetPublicProperty(string name)
        {
            foreach (var pi in PublicPropertyInfos)
            {
                if (pi.Name == name)
                    return pi;
            }
            return null;
        }

        /// <summary>
        /// Gets the public getter.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns></returns>
        public GetMemberDelegate GetPublicGetter(PropertyInfo pi) => GetPublicGetter(pi?.Name);

        /// <summary>
        /// Gets the public getter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public GetMemberDelegate GetPublicGetter(string name)
        {
            if (name == null)
                return null;

            return PropertyMap.TryGetValue(name, out PropertyAccessor info)
                ? info.PublicGetter
                : null;
        }

        /// <summary>
        /// Gets the public setter.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns></returns>
        public SetMemberDelegate GetPublicSetter(PropertyInfo pi) => GetPublicSetter(pi?.Name);

        /// <summary>
        /// Gets the public setter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public SetMemberDelegate GetPublicSetter(string name)
        {
            if (name == null)
                return null;

            return PropertyMap.TryGetValue(name, out PropertyAccessor info)
                ? info.PublicSetter
                : null;
        }
    }
}
