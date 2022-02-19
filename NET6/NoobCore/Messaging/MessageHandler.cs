using System;
using System.Linq;
using System.Threading.Tasks;
using NoobCore.Logging;

namespace NoobCore.Messaging
{
    /// <summary>
    /// Processes all messages in a Normal and Priority Queue.
    /// Expects to be called in 1 thread. i.e. Non Thread-Safe.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageHandler<T>: IMessageHandler, IDisposable
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(MessageHandler<T>));
        /// <summary>
        /// The default retry count
        /// </summary>
        public const int DefaultRetryCount = 2; //Will be a total of 3 attempts        
        /// <summary>
        /// The message service
        /// </summary>
        private readonly IMessageService messageService;
        /// <summary>
        /// The process message function
        /// </summary>
        private readonly Func<IMessage<T>, object> processMessageFn;
        /// <summary>
        /// The process in exception function
        /// </summary>
        private readonly Action<IMessageHandler, IMessage<T>, Exception> processInExceptionFn;
        /// <summary>
        /// Gets or sets the reply client factory.
        /// </summary>
        /// <value>
        /// The reply client factory.
        /// </value>
        public Func<string, IOneWayClient> ReplyClientFactory { get; set; }
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
        /// The retry count
        /// </summary>
        private readonly int retryCount;
        /// <summary>
        /// Gets the total messages processed.
        /// </summary>
        /// <value>
        /// The total messages processed.
        /// </value>
        public int TotalMessagesProcessed { get; private set; }
        /// <summary>
        /// Gets the total messages failed.
        /// </summary>
        /// <value>
        /// The total messages failed.
        /// </value>
        public int TotalMessagesFailed { get; private set; }
        /// <summary>
        /// Gets the total retries.
        /// </summary>
        /// <value>
        /// The total retries.
        /// </value>
        public int TotalRetries { get; private set; }
        /// <summary>
        /// Gets the total normal messages received.
        /// </summary>
        /// <value>
        /// The total normal messages received.
        /// </value>
        public int TotalNormalMessagesReceived { get; private set; }
        /// <summary>
        /// Gets the total priority messages received.
        /// </summary>
        /// <value>
        /// The total priority messages received.
        /// </value>
        public int TotalPriorityMessagesReceived { get; private set; }
        /// <summary>
        /// Gets the total out messages received.
        /// </summary>
        /// <value>
        /// The total out messages received.
        /// </value>
        public int TotalOutMessagesReceived { get; private set; }
        /// <summary>
        /// Gets the last message processed.
        /// </summary>
        /// <value>
        /// The last message processed.
        /// </value>
        public DateTime? LastMessageProcessed { get; private set; }

        /// <summary>
        /// Gets or sets the process queue names.
        /// </summary>
        /// <value>
        /// The process queue names.
        /// </value>
        public string[] ProcessQueueNames { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler{T}"/> class.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="processMessageFn">The process message function.</param>
        public MessageHandler(IMessageService messageService,
            Func<IMessage<T>, object> processMessageFn)
            : this(messageService, processMessageFn, null, DefaultRetryCount) { }
        /// <summary>
        /// The MqClient processing the message
        /// </summary>
        public IMessageQueueClient MqClient { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler{T}"/> class.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="processMessageFn">The process message function.</param>
        /// <param name="processInExceptionFn">The process in exception function.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <exception cref="System.ArgumentNullException">
        /// messageService
        /// or
        /// processMessageFn
        /// </exception>
        public MessageHandler(IMessageService messageService,
            Func<IMessage<T>, object> processMessageFn,
            Action<IMessageHandler, IMessage<T>, Exception> processInExceptionFn,
            int retryCount)
        {
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            this.processMessageFn = processMessageFn ?? throw new ArgumentNullException(nameof(processMessageFn));
            this.processInExceptionFn = processInExceptionFn ?? DefaultInExceptionHandler;
            this.retryCount = retryCount;
            this.ReplyClientFactory = ClientFactory.Create;
            this.ProcessQueueNames = new[] { QueueNames<T>.Priority, QueueNames<T>.In };
        }
        /// <summary>
        /// The type of the message this handler processes
        /// </summary>
        public Type MessageType => typeof(T);
        /// <summary>
        /// Process all messages pending
        /// </summary>
        /// <param name="mqClient"></param>
        public void Process(IMessageQueueClient mqClient)
        {
            foreach (var processQueueName in ProcessQueueNames)
            {
                ProcessQueue(mqClient, processQueueName);
            }
        }
        /// <summary>
        /// Process messages from a single queue.
        /// </summary>
        /// <param name="mqClient"></param>
        /// <param name="queueName">The queue to process</param>
        /// <param name="doNext">A predicate on whether to continue processing the next message if any</param>
        /// <returns></returns>
        public int ProcessQueue(IMessageQueueClient mqClient, string queueName, Func<bool> doNext = null)
        {
            var msgsProcessed = 0;
            try
            {
                IMessage<T> message;
                while ((message = mqClient.GetAsync<T>(queueName)) != null)
                {
                    ProcessMessage(mqClient, message);

                    msgsProcessed++;

                    if (doNext != null && !doNext()) 
                        return msgsProcessed;
                }
            }
            catch (TaskCanceledException) {}
            catch (Exception ex)
            {
                Log.Error("Error serializing message from mq server: " + ex.Message, ex);
            }

            return msgsProcessed;
        }
        /// <summary>
        /// Get Current Stats for this Message Handler
        /// </summary>
        /// <returns></returns>
        public IMessageHandlerStats GetStats()
        {
            return new MessageHandlerStats(typeof(T).GetOperationName(),
                TotalMessagesProcessed, TotalMessagesFailed, TotalRetries,
                TotalNormalMessagesReceived, TotalPriorityMessagesReceived, LastMessageProcessed);
        }

        /// <summary>
        /// Defaults the in exception handler.
        /// </summary>
        /// <param name="mqHandler">The mq handler.</param>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        private void DefaultInExceptionHandler(IMessageHandler mqHandler, IMessage<T> message, Exception ex)
        {
            Log.Error("Message exception handler threw an error", ex);

            bool requeue = !(ex is UnRetryableMessagingException)
                && message.RetryAttempts < retryCount;

            if (requeue)
            {
                message.RetryAttempts++;
                this.TotalRetries++;
            }

            message.Error = ex.ToResponseStatus();
            mqHandler.MqClient.Nak(message, requeue: requeue, exception: ex);
        }
        /// <summary>
        /// Process a single message
        /// </summary>
        /// <param name="mqClient"></param>
        /// <param name="mqResponse"></param>
        public void ProcessMessage(IMessageQueueClient mqClient, object mqResponse)
        {
            var message = mqClient.CreateMessage<T>(mqResponse);
            ProcessMessage(mqClient, message);
        }
        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="mqClient">The mq client.</param>
        /// <param name="message">The message.</param>
        public void ProcessMessage(IMessageQueueClient mqClient, IMessage<T> message)
        {
            this.MqClient = mqClient;
            bool msgHandled = false;

            try
            {
                var response = processMessageFn(message);
                var responseEx = response as Exception;
                
                if (responseEx != null)
                {
                    TotalMessagesFailed++;

                    msgHandled = true;
                    processInExceptionFn(this, message, responseEx);
                    return;
                }

                this.TotalMessagesProcessed++;

                //If there's no response publish the request message to its OutQ
                if (response == null)
                {
                    var publishOutqResponses = PublishToOutqWhitelist == null;
                    if (!publishOutqResponses)
                    {
                        var inWhitelist = PublishToOutqWhitelist.Contains(QueueNames<T>.Out);
                        if (!inWhitelist)
                            return;
                    }

                    var messageOptions = (MessageOption)message.Options;
                    if (messageOptions.Has(MessageOption.NotifyOneWay))
                    {
                        mqClient.Notify(QueueNames<T>.Out, message);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex is AggregateException)
                        ex = ex.UnwrapIfSingleException();
                    
                    TotalMessagesFailed++;
                    msgHandled = true;
                    processInExceptionFn(this, message, ex);
                }
                catch (Exception exHandlerEx)
                {
                    Log.Error("Message exception handler threw an error", exHandlerEx);
                }
            }
            finally
            {
                if (!msgHandled)
                    mqClient.Ack(message);

                this.TotalNormalMessagesReceived++;
                LastMessageProcessed = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var shouldDispose = messageService as IMessageHandlerDisposer;
            shouldDispose?.DisposeMessageHandler(this);
        }

    }
}