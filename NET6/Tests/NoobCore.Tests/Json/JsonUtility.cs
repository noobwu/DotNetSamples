// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 2022-11-03
//
// Last Modified By : Administrator
// Last Modified On : 2022-11-03
// ***********************************************************************
// <copyright file="JsonUtility.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Json namespace.
/// </summary>
namespace NoobCore.Tests.Json
{
    /// <summary>
    /// Class JsonUtility.
    /// </summary>
    public static class JsonUtility
    {
        /// <summary>
        /// Normalizes the json string.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>System.String.</returns>
        public static string NormalizeJsonString(string json)
        {
            JToken parsed = JToken.Parse(json);

            JToken normalized = NormalizeToken(parsed);

            return JsonConvert.SerializeObject(normalized);
        }

        /// <summary>
        /// Normalizes the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>JToken.</returns>
        private static JToken NormalizeToken(JToken token)
        {
            JObject o;
            JArray array;
            if ((o = token as JObject) != null)
            {
                List<JProperty> orderedProperties = new List<JProperty>(o.Properties());
                orderedProperties.Sort(delegate (JProperty x, JProperty y) { return x.Name.CompareTo(y.Name); });
                JObject normalized = new JObject();
                foreach (JProperty property in orderedProperties)
                {
                    normalized.Add(property.Name, NormalizeToken(property.Value));
                }
                return normalized;
            }
            else if ((array = token as JArray) != null)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    array[i] = NormalizeToken(array[i]);
                }
                return array;
            }
            else
            {
                return token;
            }
        }
    }
}
