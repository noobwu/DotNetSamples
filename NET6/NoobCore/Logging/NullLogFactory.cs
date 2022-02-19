using System;

namespace NoobCore.Logging
{
    /// <summary>
    /// Creates a Debug Logger, that logs all messages to: System.Diagnostics.Debug
    /// 
    /// Made public so its testable
    /// </summary>
	public class NullLogFactory : ILogFactory
    {
        /// <summary>
        /// The debug enabled
        /// </summary>
        private readonly bool debugEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullLogFactory"/> class.
        /// </summary>
        /// <param name="debugEnabled">if set to <c>true</c> [debug enabled].</param>
        public NullLogFactory(bool debugEnabled=false)
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
			return new NullDebugLogger(type) { IsDebugEnabled = debugEnabled };
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ILog GetLogger(string typeName)
        {
            return new NullDebugLogger(typeName) { IsDebugEnabled = debugEnabled };
        }
    }
}
