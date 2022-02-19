using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using static System.String;
using System.Collections.Concurrent;
using NoobCore.Text;

namespace NoobCore
{
    /// <summary>
    /// Donated by Ivan Korneliuk from his post:
    /// http://korneliuk.blogspot.com/2012/08/servicestack-reusing-dtos.html
    /// 
    /// Modified to only allow using routes matching the supplied HTTP Verb
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Gets the name of the operation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetOperationName(this Type type)
        {
            //Need to expand Arrays of Generic Types like Nullable<Byte>[]
            if (type.IsArray)
            {
                return type.GetElementType().ExpandTypeName() + "[]";
            }

            string fullname = type.FullName;
            int genericPrefixIndex = type.IsGenericParameter ? 1 : 0;

            if (fullname == null)
                return genericPrefixIndex > 0 ? "'" + type.Name : type.Name;

            int startIndex = type.Namespace != null ? type.Namespace.Length + 1 : 0; //trim namespace + "."
            int endIndex = fullname.IndexOf("[[", startIndex, StringComparison.Ordinal);  //Generic Fullname
            if (endIndex == -1)
                endIndex = fullname.Length;

            char[] op = new char[endIndex - startIndex + genericPrefixIndex];

            for (int i = startIndex; i < endIndex; i++)
            {
                var cur = fullname[i];
                op[i - startIndex + genericPrefixIndex] = cur != '+' ? cur : '.';
            }

            if (genericPrefixIndex > 0)
                op[0] = '\'';

            return new string(op);
        }
        /// <summary>
        /// Expands the name of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string ExpandTypeName(this Type type)
        {
            if (type.IsGenericType)
                return ExpandGenericTypeName(type);

            return type.GetOperationName();
        }
        /// <summary>
        /// Expands the name of the generic type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string ExpandGenericTypeName(Type type)
        {
            var nameOnly = type.Name.LeftPart('`');

            var sb = StringBuilderCache.Allocate();
            foreach (var arg in type.GetGenericArguments())
            {
                if (sb.Length > 0)
                    sb.Append(',');

                sb.Append(arg.ExpandTypeName());
            }

            var fullName = $"{nameOnly}<{StringBuilderCache.ReturnAndFree(sb)}>";
            return fullName;
        }
    }
}