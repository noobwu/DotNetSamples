using System;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum MessageOption : int
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// All
        /// </summary>
        All = int.MaxValue,
        /// <summary>
        /// The notify one way
        /// </summary>
        NotifyOneWay = 1 << 0,
    }
}