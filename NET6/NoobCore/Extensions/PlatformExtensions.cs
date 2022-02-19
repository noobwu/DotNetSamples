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
    }
}
