using System;

namespace NoobCore.Logging
{
    /// <summary>
    /// Creates a Debug Logger, that logs all messages to: System.Diagnostics.Debug
    /// 
    /// Made public so its testable
    /// </summary>
    public class DebugLogFactory : ILogFactory
    {
        /// <summary>
        /// The debug enabled
        /// </summary>
        private readonly bool debugEnabled;
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogFactory"/> class.
        /// </summary>
        /// <param name="debugEnabled">if set to <c>true</c> [debug enabled].</param>
        public DebugLogFactory(bool debugEnabled = true)
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
            return new DebugLogger(type) { IsDebugEnabled = debugEnabled };
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ILog GetLogger(string typeName)
        {
            return new DebugLogger(typeName) { IsDebugEnabled = debugEnabled };
        }
    }
}
