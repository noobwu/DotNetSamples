using System;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IMessageProducer: IDisposable
    {
        /// <summary>
        /// Publishes the specified message body.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageBody">The message body.</param>
        void Publish<T>(T messageBody);
        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        void Publish<T>(IMessage<T> message);
    }

}
