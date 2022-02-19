using System;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IMessageQueueClientFactory
        : IDisposable
    {
        /// <summary>
        /// Creates the message queue client.
        /// </summary>
        /// <returns></returns>
        IMessageQueueClient CreateMessageQueueClient();
    }
}