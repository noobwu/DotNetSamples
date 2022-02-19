using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class Env
    {
        /// <summary>
        /// Gets the current application user model identifier.
        /// </summary>
        /// <param name="applicationUserModelIdLength">Length of the application user model identifier.</param>
        /// <param name="applicationUserModelId">The application user model identifier.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern int GetCurrentApplicationUserModelId(ref uint applicationUserModelIdLength, byte[] applicationUserModelId);
        /// <summary>
        /// The continue on captured context
        /// </summary>
        public const bool ContinueOnCapturedContext = false;

        /// <summary>
        /// Configurations the await.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfiguredTaskAwaitable ConfigAwait(this Task task) =>
            task.ConfigureAwait(ContinueOnCapturedContext);
        /// <summary>
        /// Configurations the await.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfiguredTaskAwaitable<T> ConfigAwait<T>(this Task<T> task) =>
            task.ConfigureAwait(ContinueOnCapturedContext);

        /// <summary>
        /// Configurations the await.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfiguredValueTaskAwaitable ConfigAwait(this ValueTask task) => 
            task.ConfigureAwait(ContinueOnCapturedContext);
        /// <summary>
        /// Configurations the await.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfiguredValueTaskAwaitable<T> ConfigAwait<T>(this ValueTask<T> task) => 
            task.ConfigureAwait(ContinueOnCapturedContext);
    }
}
