#if !NETFX_CORE
using System;

namespace NoobCore.Logging
{
    /// <summary>
    /// Creates a Console Logger, that logs all messages to: System.Console
    /// 
    /// Made public so its testable
    /// </summary>
    public class ConsoleLogFactory : ILogFactory
    {
        /// <summary>
        /// The debug enabled
        /// </summary>
        private readonly bool debugEnabled;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogFactory"/> class.
        /// </summary>
        /// <param name="debugEnabled">if set to <c>true</c> [debug enabled].</param>
        public ConsoleLogFactory(bool debugEnabled = true)
        {
            this.debugEnabled = debugEnabled;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILog GetLogger(Type type)
        {
            return new ConsoleLogger(type) { IsDebugEnabled = debugEnabled };
        }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ILog GetLogger(string typeName)
        {
            return new ConsoleLogger(typeName) { IsDebugEnabled = debugEnabled };
        }
        /// <summary>
        /// Configures the specified debug enabled.
        /// </summary>
        /// <param name="debugEnabled">if set to <c>true</c> [debug enabled].</param>
        public static void Configure(bool debugEnabled = true)
        {
            LogManager.LogFactory = new ConsoleLogFactory(); 
        }
    }
}
#endif
