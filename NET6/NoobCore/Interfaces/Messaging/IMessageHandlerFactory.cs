namespace NoobCore.Messaging
{
    /// <summary>
    /// Encapsulates creating a new message handler
    /// </summary>
    public interface IMessageHandlerFactory
    {
        /// <summary>
        /// Creates the message handler.
        /// </summary>
        /// <returns></returns>
        IMessageHandler CreateMessageHandler();
    }
}