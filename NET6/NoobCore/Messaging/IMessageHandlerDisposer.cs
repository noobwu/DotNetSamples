namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageHandlerDisposer
    {
        /// <summary>
        /// Disposes the message handler.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        void DisposeMessageHandler(IMessageHandler messageHandler);
    }
}