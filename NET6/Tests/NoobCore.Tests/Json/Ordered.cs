// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 2022-11-03
//
// Last Modified By : Administrator
// Last Modified On : 2022-11-03
// ***********************************************************************
// <copyright file="Ordered.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Json namespace.
/// </summary>
namespace NoobCore.Tests.Json
{
    /// <summary>
    /// Class OrderedPropertiesContractResolver.
    /// Implements the <see cref="DefaultContractResolver" />
    /// </summary>
    /// <seealso cref="DefaultContractResolver" />
    public class OrderedPropertiesContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>
        /// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
        protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            return props.OrderBy(p => p.PropertyName).ToList();
        }

        /// <summary>
        /// Class OrderedExpandoPropertiesConverter.
        /// Implements the <see cref="ExpandoObjectConverter" />
        /// </summary>
        /// <seealso cref="ExpandoObjectConverter" />
        public class OrderedExpandoPropertiesConverter : ExpandoObjectConverter
        {
            /// <summary>
            /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
            /// </summary>
            /// <value><c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.</value>
            public override bool CanWrite
            {
                get { return true; }
            }

            /// <summary>
            /// Writes the JSON representation of the object.
            /// </summary>
            /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var expando = (IDictionary<string, object>)value;
                var orderedDictionary = expando.OrderBy(x => x.Key).ToDictionary(t => t.Key, t => t.Value);
                serializer.Serialize(writer, orderedDictionary);
            }
        }


        /// <summary>
        /// Class MetadataTokenContractResolver.
        /// Implements the <see cref="DefaultContractResolver" />
        /// </summary>
        /// <seealso cref="DefaultContractResolver" />
        public class MetadataTokenContractResolver : DefaultContractResolver
        {
            /// <summary>
            /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
            /// </summary>
            /// <param name="type">The type to create properties for.</param>
            /// <param name="memberSerialization">The member serialization mode for the type.</param>
            /// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
            protected override IList<JsonProperty> CreateProperties(
                Type type, MemberSerialization memberSerialization)
            {
                var props = type
                   .GetProperties(BindingFlags.Instance
                       | BindingFlags.Public
                       | BindingFlags.NonPublic
                   ).ToDictionary(k => k.Name, v =>
                   {
                       // first value: declaring type
                       var classIndex = 0;
                       var t = type;
                       while (t != v.DeclaringType)
                       {
                           classIndex++;
                           t = type.BaseType;
                       }
                       return Tuple.Create(classIndex, v.MetadataToken);
                   });

                return base.CreateProperties(type, memberSerialization)
                    .OrderByDescending(p => props[p.PropertyName].Item1)
                    .ThenBy(p => props[p.PropertyName].Item1)
                    .ToList();
            }
        }
    }
}
