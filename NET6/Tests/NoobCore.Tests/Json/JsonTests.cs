// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 2022-11-03
//
// Last Modified By : Administrator
// Last Modified On : 2022-11-03
// ***********************************************************************
// <copyright file="JsonTests.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NoobCore.Tests.Json.OrderedPropertiesContractResolver;

/// <summary>
/// The Json namespace.
/// </summary>
namespace NoobCore.Tests.Json
{
    /// <summary>
    /// Class JsonTests.
    /// </summary>
    [TestFixture]
    public class JsonTests
    {
        /// <summary>
        /// Defines the test method SerializeObject.
        /// </summary>
        [TestCase]
        public void SerializeObject()
        {
            var d = new Dictionary<string, int>
            {
                ["a"] = 1,
                ["c"] = 2,
                ["b"] = 3,
            };

            var settings = new JsonSerializerSettings { ContractResolver = SortedPropertiesContractResolver.Instance };
            var json = JsonConvert.SerializeObject(d, Formatting.Indented, settings);
            Console.WriteLine($"SerializeObject:{json}");

            var s = new JsonSerializerSettings
            {
                ContractResolver = new OrderedPropertyNamesContractResolver(StringComparer.OrdinalIgnoreCase),
            };
            Console.WriteLine($"SerializeObject:{JsonConvert.SerializeObject(d)}"); // yields {"a":1,"c":2,"b":3}, as expected.
            Console.WriteLine($"OrderedSerializeObject:{JsonConvert.SerializeObject(d, s)}");// yields the same, but was hoping for {"a":1,"b":3,"c":2}
            
            settings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedPropertiesContractResolver(),
                Converters = { new OrderedExpandoPropertiesConverter() }
            };
             json = JsonConvert.SerializeObject(d, Formatting.Indented, settings);
            Console.WriteLine($"SerializeObject:{json}");

             settings = new JsonSerializerSettings()
            {
                ContractResolver = new MetadataTokenContractResolver(),
            };
            json = JsonConvert.SerializeObject(d, Formatting.Indented, settings);
            Console.WriteLine($"SerializeObject:{json}");
        }
        /// <summary>
        /// Class SortedPropertiesContractResolver.
        /// Implements the <see cref="DefaultContractResolver" />
        /// </summary>
        /// <seealso cref="DefaultContractResolver" />
        private class SortedPropertiesContractResolver : DefaultContractResolver
        {

            // use a static instance for optimal performance
            static SortedPropertiesContractResolver instance;

            /// <summary>
            /// Initializes static members of the <see cref="SortedPropertiesContractResolver"/> class.
            /// </summary>
            static SortedPropertiesContractResolver() { instance = new SortedPropertiesContractResolver(); }

            /// <summary>
            /// Gets the instance.
            /// </summary>
            /// <value>The instance.</value>
            public static SortedPropertiesContractResolver Instance { get { return instance; } }

            /// <summary>
            /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
            /// </summary>
            /// <param name="type">The type to create properties for.</param>
            /// <param name="memberSerialization">The member serialization mode for the type.</param>
            /// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);
                if (properties != null)
                    return properties.OrderBy(p => p.UnderlyingName).ToList();
                return properties;
            }
        }
    }

    // My very naive attempt at doing this without constructing a whole new SortedDictionary or SortedList ---   
    /// <summary>
    /// Class OrderedPropertyNamesContractResolver.
    /// Implements the <see cref="DefaultContractResolver" />
    /// </summary>
    /// <seealso cref="DefaultContractResolver" />
    public class OrderedPropertyNamesContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// The comparer
        /// </summary>
        readonly IComparer<string> Comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedPropertyNamesContractResolver"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="System.ArgumentNullException">comparer</exception>
        public OrderedPropertyNamesContractResolver(IComparer<string> comparer) => Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

        /// <summary>
        /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>
        /// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization).OrderBy(property => property.PropertyName, Comparer).ToList();
            foreach (var (property, index) in properties.Select(ValueTuple.Create<JsonProperty, int>))
            {
                property.Order = index;
            }
            return properties;
        }
    }
}
