using System;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.Messaging.IMessageQueueClientFactory" />
    public interface IMessageFactory : IMessageQueueClientFactory
    {
        /// <summary>
        /// Creates the message producer.
        /// </summary>
        /// <returns></returns>
        IMessageProducer CreateMessageProducer();
    }
}