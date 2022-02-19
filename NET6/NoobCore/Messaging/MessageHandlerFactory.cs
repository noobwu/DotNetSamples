using System;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="NoobCore.Messaging.IMessageHandlerFactory" />
    public class MessageHandlerFactory<T>: IMessageHandlerFactory
    {
        /// <summary>
        /// The default retry count
        /// </summary>
        public const int DefaultRetryCount = 2; //Will be a total of 3 attempts        
        /// <summary>
        /// The message service
        /// </summary>
        private readonly IMessageService messageService;

        /// <summary>
        /// Gets or sets the request filter.
        /// </summary>
        /// <value>
        /// The request filter.
        /// </value>
        public Func<IMessage, IMessage> RequestFilter { get; set; }
        /// <summary>
        /// Gets or sets the response filter.
        /// </summary>
        /// <value>
        /// The response filter.
        /// </value>
        public Func<object, object> ResponseFilter { get; set; }
        /// <summary>
        /// Gets or sets the publish responses whitelist.
        /// </summary>
        /// <value>
        /// The publish responses whitelist.
        /// </value>
        public string[] PublishResponsesWhitelist { get; set; }
        /// <summary>
        /// Gets or sets the publish to outq whitelist.
        /// </summary>
        /// <value>
        /// The publish to outq whitelist.
        /// </value>
        public string[] PublishToOutqWhitelist { get; set; }

        /// <summary>
        /// The process message function
        /// </summary>
        private readonly Func<IMessage<T>, object> processMessageFn;
        /// <summary>
        /// The process exception function
        /// </summary>
        private readonly Action<IMessageHandler, IMessage<T>, Exception> processExceptionFn;
        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>
        /// The retry count.
        /// </value>
        public int RetryCount { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactory{T}"/> class.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="processMessageFn">The process message function.</param>
        public MessageHandlerFactory(IMessageService messageService, Func<IMessage<T>, object> processMessageFn)
            : this(messageService, processMessageFn, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactory{T}"/> class.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="processMessageFn">The process message function.</param>
        /// <param name="processExceptionEx">The process exception ex.</param>
        /// <exception cref="System.ArgumentNullException">
        /// messageService
        /// or
        /// processMessageFn
        /// </exception>
        public MessageHandlerFactory(IMessageService messageService,
            Func<IMessage<T>, object> processMessageFn,
            Action<IMessageHandler, IMessage<T>, Exception> processExceptionEx)
        {
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            this.processMessageFn = processMessageFn ?? throw new ArgumentNullException(nameof(processMessageFn));
            this.processExceptionFn = processExceptionEx;
            this.RetryCount = DefaultRetryCount;
        }
        /// <summary>
        /// Creates the message handler.
        /// </summary>
        /// <returns></returns>
        public IMessageHandler CreateMessageHandler()
        {
            if (this.RequestFilter == null && this.ResponseFilter == null)
            {
                return new MessageHandler<T>(messageService, processMessageFn, processExceptionFn, this.RetryCount)
                {
                    PublishResponsesWhitelist = PublishResponsesWhitelist,
                    PublishToOutqWhitelist = PublishToOutqWhitelist,
                };
            }

            return new MessageHandler<T>(messageService, msg =>
                {
                    if (this.RequestFilter != null)
                        msg = (IMessage<T>)this.RequestFilter(msg);

                    var result = this.processMessageFn(msg);

                    if (this.ResponseFilter != null)
                        result = this.ResponseFilter(result);

                    return result;
                },
                processExceptionFn, this.RetryCount)
            {
                PublishResponsesWhitelist = PublishResponsesWhitelist,
                PublishToOutqWhitelist = PublishToOutqWhitelist,
            };
        }
    }
}