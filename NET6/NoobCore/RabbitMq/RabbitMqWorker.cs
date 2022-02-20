using System;
using System.IO;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using NoobCore.Logging;
using NoobCore.Messaging;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace NoobCore.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class RabbitMqWorker : IDisposable
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(RabbitMqWorker));

        /// <summary>
        /// The MSG lock
        /// </summary>
        readonly object msgLock = new object();

        /// <summary>
        /// The mq factory
        /// </summary>
        private readonly RabbitMqMessageFactory mqFactory;
        /// <summary>
        /// The mq client
        /// </summary>
        private IMessageQueueClient mqClient;
        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IMessageHandler messageHandler;
        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string QueueName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [automatic reconnect].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic reconnect]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoReconnect { get; set; }
        /// <summary>
        /// The status
        /// </summary>
        private int status;
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public int Status => status;

        /// <summary>
        /// The bg thread
        /// </summary>
        private Thread bgThread;
        /// <summary>
        /// The times started
        /// </summary>
        private int timesStarted = 0;
        /// <summary>
        /// The received new MSGS
        /// </summary>
        private bool receivedNewMsgs = false;
        /// <summary>
        /// The sleep timeout ms
        /// </summary>
        public int SleepTimeoutMs = 1000;
        /// <summary>
        /// Gets or sets the error handler.
        /// </summary>
        /// <value>
        /// The error handler.
        /// </value>
        public Action<RabbitMqWorker, Exception> errorHandler { get; set; }
        /// <summary>
        /// The last MSG processed
        /// </summary>
        private DateTime lastMsgProcessed;
        /// <summary>
        /// Gets the last MSG processed.
        /// </summary>
        /// <value>
        /// The last MSG processed.
        /// </value>
        public DateTime LastMsgProcessed => lastMsgProcessed;
        /// <summary>
        /// The total messages processed
        /// </summary>
        private int totalMessagesProcessed;
        /// <summary>
        /// Gets the total messages processed.
        /// </summary>
        /// <value>
        /// The total messages processed.
        /// </value>
        public int TotalMessagesProcessed => totalMessagesProcessed;

        // TODO: RabbitMqWorker.MsgNotificationsReceived is never referenced and will always return zero.
        //private int msgNotificationsReceived;        
        /// <summary>
        /// Gets the MSG notifications received.
        /// </summary>
        /// <value>
        /// The MSG notifications received.
        /// </value>
        public int MsgNotificationsReceived => 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqWorker"/> class.
        /// </summary>
        /// <param name="mqFactory">The mq factory.</param>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="errorHandler">The error handler.</param>
        /// <param name="autoConnect">if set to <c>true</c> [automatic connect].</param>
        public RabbitMqWorker(RabbitMqMessageFactory mqFactory,
            IMessageHandler messageHandler, string queueName,
            Action<RabbitMqWorker, Exception> errorHandler,
            bool autoConnect = true)
        {
            this.mqFactory = mqFactory;
            this.messageHandler = messageHandler;
            this.QueueName = queueName;
            this.errorHandler = errorHandler;
            this.AutoReconnect = autoConnect;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public virtual RabbitMqWorker Clone()
        {
            return new RabbitMqWorker(mqFactory, messageHandler, QueueName, errorHandler, AutoReconnect);
        }

        /// <summary>
        /// Gets the mq client.
        /// </summary>
        /// <value>
        /// The mq client.
        /// </value>
        public IMessageQueueClient MqClient => mqClient ?? (mqClient = mqFactory.CreateMessageQueueClient());

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <returns></returns>
        private IModel GetChannel()
        {
            var rabbitClient = (RabbitMqQueueClient)MqClient;
            var channel = rabbitClient.Channel;
            return channel;
        }
        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">MQ Host has been disposed</exception>
        public virtual void Start()
        {
            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Started)
                return;
            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Disposed)
                throw new ObjectDisposedException("MQ Host has been disposed");
            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Stopping)
                KillBgThreadIfExists();

            if (Interlocked.CompareExchange(ref status, WorkerStatus.Starting, WorkerStatus.Stopped) == WorkerStatus.Stopped)
            {
                Log.Debug($"Starting MQ Handler Worker: {QueueName}...");

                //Should only be 1 thread past this point
                bgThread = new Thread(Run)
                {
                    Name = $"{GetType().Name}: {QueueName}",
                    IsBackground = true,
                };
                bgThread.Start();
            }
        }
        /// <summary>
        /// Forces the restart.
        /// </summary>
        public virtual void ForceRestart()
        {
            KillBgThreadIfExists();
            Start();
        }
        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            if (Interlocked.CompareExchange(ref status, WorkerStatus.Started, WorkerStatus.Starting) != WorkerStatus.Starting) return;
            timesStarted++;

            try
            {
                if (mqFactory.UsePolling)
                {
                    lock (msgLock)
                    {
                        StartPolling();
                    }
                }
                else
                {
                    StartSubscription();
                }
            }
            catch (Exception ex)
            {
#if !NETCORE
                //Ignore handling rare, but expected exceptions from KillBgThreadIfExists()
                if (ex is ThreadInterruptedException || ex is ThreadAbortException)
                {
                    Log.Warn($"Received {ex.GetType().Name} in Worker: {QueueName}");
                    return;
                }
#endif

                Stop();

                errorHandler?.Invoke(this, ex);
            }
            finally
            {
                try
                {
                    DisposeMqClient();
                }
                catch { }

                //If it's in an invalid state, Dispose() this worker.
                if (Interlocked.CompareExchange(ref status, WorkerStatus.Stopped, WorkerStatus.Stopping) != WorkerStatus.Stopping)
                {
                    Dispose();
                }
                //status is either 'Stopped' or 'Disposed' at this point

                bgThread = null;
            }
        }


        /// <summary>
        /// Starts the polling.
        /// </summary>
        private void StartPolling()
        {
            while (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Started)
            {
                try
                {
                    receivedNewMsgs = false;

                    var msgsProcessedThisTime = messageHandler.ProcessQueue(
                        MqClient, QueueName,
                        () => Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Started);

                    totalMessagesProcessed += msgsProcessedThisTime;

                    if (msgsProcessedThisTime > 0)
                        lastMsgProcessed = DateTime.UtcNow;

                    if (!receivedNewMsgs)
                        Monitor.Wait(msgLock, millisecondsTimeout: SleepTimeoutMs);
                }
                catch (Exception ex)
                {
                    if (!(ex is OperationInterruptedException
                        || ex is EndOfStreamException))
                        throw;

                    //The consumer was cancelled, the model or the connection went away.
                    if (Interlocked.CompareExchange(ref status, 0, 0) != WorkerStatus.Started
                        || !AutoReconnect)
                        return;

                    //If it was an unexpected exception, try reconnecting
                    WaitForReconnect();
                }
            }
        }

        /// <summary>
        /// Waits for reconnect.
        /// </summary>
        /// <returns></returns>
        private IModel WaitForReconnect()
        {
            var retries = 1;
            while (true)
            {
                DisposeMqClient();
                try
                {
                    var channel = GetChannel();
                    return channel;
                }
                catch (Exception ex)
                {
                    var waitMs = Math.Min(retries++ * 100, 10000);
                    Log.Debug($"Retrying to Reconnect after {waitMs}ms...", ex);
                    Thread.Sleep(waitMs);
                }
            }
        }
        /// <summary>
        /// Starts the subscription.
        /// </summary>
        private void StartSubscription()
        {
            using (var barrier = new ManualResetEventSlim(false))
            {
                var consumer = ConnectSubscription();
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var basicGetResult = new BasicGetResult(ea.DeliveryTag, ea.Redelivered, ea.Exchange, ea.RoutingKey, 0, ea.BasicProperties, ea.Body);
                        messageHandler.ProcessMessage(mqClient, basicGetResult);
                        mqFactory.GetMessageFilter?.Invoke(QueueName, basicGetResult);
                     
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is OperationInterruptedException
                            || ex is EndOfStreamException))
                            throw;

                        // The consumer was cancelled, the model closed, or the connection went away.
                        if (Interlocked.CompareExchange(ref status, 0, 0) != WorkerStatus.Started
                            || !AutoReconnect)
                            return;
                        //If it was an unexpected exception, try reconnecting
                        WaitForReconnectStartSubscription();
                    }
                    finally
                    {
                        barrier.Set(); // Signal Event fired.
                    }
                };
                barrier.Wait(); // Wait for Event to fire.
            }

        }

        /// <summary>
        /// Waits for reconnect subscription.
        /// </summary>
        /// <returns></returns>
        private RabbitMqBasicConsumer WaitForReconnectSubscription()
        {
            var retries = 1;
            while (true)
            {
                DisposeMqClient();
                try
                {
                    return ConnectSubscription();
                }
                catch (Exception ex)
                {
                    var waitMs = Math.Min(retries++ * 100, 10000);
                    Log.Debug($"Retrying to Reconnect Subscription after {waitMs}ms...", ex);
                    Thread.Sleep(waitMs);
                }
            }
        }
        /// <summary>
        /// Waits for reconnect start subscription.
        /// </summary>
        private void WaitForReconnectStartSubscription()
        {
            var retries = 1;
            while (true)
            {
                DisposeMqClient();
                try
                {
                    StartSubscription();
                }
                catch (Exception ex)
                {
                    var waitMs = Math.Min(retries++ * 100, 10000);
                    Log.Debug($"Retrying to Reconnect Start Subscription after {waitMs}ms...", ex);
                    Thread.Sleep(waitMs);
                }
            }
        }
        /// <summary>
        /// Connects the subscription.
        /// </summary>
        /// <returns></returns>
        private RabbitMqBasicConsumer ConnectSubscription()
        {
            var channel = GetChannel();
            var consumer = new RabbitMqBasicConsumer(channel);
            channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);
            return consumer;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public virtual void Stop()
        {
            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Disposed)
                return;

            if (Interlocked.CompareExchange(ref status, WorkerStatus.Stopping, WorkerStatus.Started) == WorkerStatus.Started)
            {
                Log.Debug($"Stopping Rabbit MQ Handler Worker: {QueueName}...");
                if (mqFactory.UsePolling)
                {
                    Thread.Sleep(100);
                    lock (msgLock)
                    {
                        Monitor.Pulse(msgLock);
                    }
                }

                DisposeMqClient();
            }
        }

        /// <summary>
        /// Disposes the mq client.
        /// </summary>
        private void DisposeMqClient()
        {
            //Disposing mqClient causes an EndOfStreamException to be thrown in StartSubscription
            if (mqClient == null) return;
            mqClient.Dispose();
            mqClient = null;
        }

        /// <summary>
        /// Kills the bg thread if exists.
        /// </summary>
        private void KillBgThreadIfExists()
        {
            try
            {
                if (bgThread != null && bgThread.IsAlive)
                {
                    //give it a small chance to die gracefully
                    if (!bgThread.Join(500))
                    {
                        //Ideally we shouldn't get here, but lets try our hardest to clean it up
                        Log.Warn("Interrupting previous Background Worker: " + bgThread.Name);
                        bgThread.Interrupt();
                        if (!bgThread.Join(TimeSpan.FromSeconds(3)))
                        {
                            Log.Warn(bgThread.Name + " just wont die, so we're now aborting it...");
                            bgThread.Abort();
                        }
                    }
                }
            }
            finally
            {
                bgThread = null;
                Interlocked.CompareExchange(ref status, WorkerStatus.Stopped, status);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Disposed)
                return;

            Stop();

            if (Interlocked.CompareExchange(ref status, WorkerStatus.Disposed, WorkerStatus.Stopped) != WorkerStatus.Stopped)
                Interlocked.CompareExchange(ref status, WorkerStatus.Disposed, WorkerStatus.Stopping);

            try
            {
                KillBgThreadIfExists();
            }
            catch (Exception ex)
            {
                Log.Error("Error Disposing MessageHandlerWorker for: " + QueueName, ex);
            }
        }

        /// <summary>
        /// Gets the stats.
        /// </summary>
        /// <returns></returns>
        public virtual IMessageHandlerStats GetStats()
        {
            return messageHandler.GetStats();
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <returns></returns>
        public virtual string GetStatus()
        {
            return $"[Worker: {QueueName}, Status: {WorkerStatus.ToString(status)}, ThreadStatus: {bgThread.ThreadState}, LastMsgAt: {LastMsgProcessed}]";
        }
    }
}