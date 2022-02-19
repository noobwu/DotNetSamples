// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Reflection;

namespace NoobCore
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetDescription(this Type type)
        {
            var componentDescAttr = type.FirstAttribute<System.ComponentModel.DescriptionAttribute>();
            if (componentDescAttr == null)
            {
                return string.Empty;
            }
            return componentDescAttr.Description;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="mi">The mi.</param>
        /// <returns></returns>
        public static string GetDescription(this MemberInfo mi)
        {
            var componentDescAttr = mi.FirstAttribute<System.ComponentModel.DescriptionAttribute>();
            if (componentDescAttr == null)
            {
                return string.Empty;
            }
            return componentDescAttr.Description;

        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns></returns>
        public static string GetDescription(this ParameterInfo pi)
        {
            var componentDescAttr = pi.FirstAttribute<System.ComponentModel.DescriptionAttribute>();
            if (componentDescAttr == null) {
                return string.Empty;
            }
            return componentDescAttr.Description;
        }
    }
}
