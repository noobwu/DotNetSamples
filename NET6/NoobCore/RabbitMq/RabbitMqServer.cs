using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NoobCore.Logging;
using NoobCore.Messaging;
using NoobCore.Text;
using RabbitMQ.Client;

namespace NoobCore.RabbitMq
{
    public class RabbitMqServer : IMessageService
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(RabbitMqServer));
        /// <summary>
        /// The default retry count
        /// </summary>
        public const int DefaultRetryCount = 1; //Will be a total of 2 attempts

        /// <summary>
        /// The RabbitMQ.Client Connection factory to introspect connection properties and create a low-level connection
        /// </summary>
        public ConnectionFactory ConnectionFactory => messageFactory.ConnectionFactory;

        /// <summary>
        /// Whether Rabbit MQ should auto-retry connecting when a connection to Rabbit MQ Server instance is dropped
        /// </summary>
        public bool AutoReconnect { get; set; }

        /// <summary>
        /// How many times a message should be retried before sending to the DLQ (Max of 1).
        /// </summary>
        public int RetryCount
        {
            get => messageFactory.RetryCount;
            set => messageFactory.RetryCount = value;
        }

        /// <summary>
        /// Whether to use polling for consuming messages instead of a long-term subscription
        /// </summary>
        public bool UsePolling
        {
            get => messageFactory.UsePolling;
            set => messageFactory.UsePolling = value;
        }

        /// <summary>
        /// Wait before Starting the MQ Server after a restart 
        /// </summary>
        public int? KeepAliveRetryAfterMs { get; set; }

        /// <summary>
        /// The Message Factory used by this MQ Server
        /// </summary>
        private RabbitMqMessageFactory messageFactory;
        /// <summary>
        /// Factory to create consumers and producers that work with this service
        /// </summary>
        public IMessageFactory MessageFactory => messageFactory;

        /// <summary>
        /// Gets or sets the mq queue client filter.
        /// </summary>
        /// <value>
        /// The mq queue client filter.
        /// </value>
        public Action<RabbitMqQueueClient> MqQueueClientFilter
        {
            get => messageFactory.MqQueueClientFilter;
            set => messageFactory.MqQueueClientFilter = value;
        }

        /// <summary>
        /// Gets or sets the mq producer filter.
        /// </summary>
        /// <value>
        /// The mq producer filter.
        /// </value>
        public Action<RabbitMqProducer> MqProducerFilter
        {
            get => messageFactory.MqProducerFilter;
            set => messageFactory.MqProducerFilter = value;
        }

        /// <summary>
        /// Gets or sets the publish message filter.
        /// </summary>
        /// <value>
        /// The publish message filter.
        /// </value>
        public Action<string, IBasicProperties, IMessage> PublishMessageFilter
        {
            get => messageFactory.PublishMessageFilter;
            set => messageFactory.PublishMessageFilter = value;
        }

        /// <summary>
        /// Gets or sets the get message filter.
        /// </summary>
        /// <value>
        /// The get message filter.
        /// </value>
        public Action<string, BasicGetResult> GetMessageFilter
        {
            get => messageFactory.GetMessageFilter;
            set => messageFactory.GetMessageFilter = value;
        }

        /// <summary>
        /// Gets or sets the create queue filter.
        /// </summary>
        /// <value>
        /// The create queue filter.
        /// </value>
        public Action<string, Dictionary<string,object>> CreateQueueFilter { get; set; }
        /// <summary>
        /// Gets or sets the create topic filter.
        /// </summary>
        /// <value>
        /// The create topic filter.
        /// </value>
        public Action<string, Dictionary<string,object>> CreateTopicFilter { get; set; }

        /// <summary>
        /// Execute global transformation or custom logic before a request is processed.
        /// Must be thread-safe.
        /// </summary>
        public Func<IMessage, IMessage> RequestFilter { get; set; }

        /// <summary>
        /// Execute global transformation or custom logic on the response.
        /// Must be thread-safe.
        /// </summary>
        public Func<object, object> ResponseFilter { get; set; }

        /// <summary>
        /// Execute global error handler logic. Must be thread-safe.
        /// </summary>
        public Action<Exception> ErrorHandler { get; set; }

        /// <summary>
        /// If you only want to enable priority queue handlers (and threads) for specific msg types
        /// </summary>
        public string[] PriorityQueuesWhitelist { get; set; }

        /// <summary>
        /// Don't listen on any Priority Queues
        /// </summary>
        public bool DisablePriorityQueues
        {
            set => PriorityQueuesWhitelist = TypeConstants.EmptyStringArray;
        }

        /// <summary>
        /// Opt-in to only publish responses on this white list. 
        /// Publishes all responses by default.
        /// </summary>
        public string[] PublishResponsesWhitelist { get; set; }

        /// <summary>
        /// Don't publish any response messages
        /// </summary>
        public bool DisablePublishingResponses
        {
            set => PublishResponsesWhitelist = value ? TypeConstants.EmptyStringArray : null;
        }

        /// <summary>
        /// Opt-in to only publish .outq messages on this white list. 
        /// Publishes all responses by default.
        /// </summary>
        public string[] PublishToOutqWhitelist { get; set; }

        /// <summary>
        /// Don't publish any messages to .outq
        /// </summary>
        public bool DisablePublishingToOutq
        {
            set => PublishToOutqWhitelist = value ? TypeConstants.EmptyStringArray : null;
        }

        /// <summary>
        /// The connection
        /// </summary>
        private IConnection connection;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        private IConnection Connection => connection ??= ConnectionFactory.CreateConnection();

        /// <summary>
        /// The handler map
        /// </summary>
        private readonly Dictionary<Type, IMessageHandlerFactory> handlerMap
            = new Dictionary<Type, IMessageHandlerFactory>();
        /// <summary>
        /// The handler thread count map
        /// </summary>
        private readonly Dictionary<Type, int> handlerThreadCountMap
            = new Dictionary<Type, int>();
        /// <summary>
        /// The workers
        /// </summary>
        private RabbitMqWorker[] workers;
        /// <summary>
        /// The queue worker index map
        /// </summary>
        private Dictionary<string, int[]> queueWorkerIndexMap;

        /// <summary>
        /// Get a list of all message types registered on this MQ Host
        /// </summary>
        public List<Type> RegisteredTypes => handlerMap.Keys.ToList();

        public RabbitMqServer(string connectionString="localhost",string username = null, string password = null)
        {
            Init(new RabbitMqMessageFactory(connectionString, username, password));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqServer"/> class.
        /// </summary>
        /// <param name="messageFactory">The message factory.</param>
        public RabbitMqServer(RabbitMqMessageFactory messageFactory)
        {
            Init(messageFactory);
        }

        /// <summary>
        /// Initializes the specified message factory.
        /// </summary>
        /// <param name="messageFactory">The message factory.</param>
        private void Init(RabbitMqMessageFactory messageFactory)
        {
            this.messageFactory = messageFactory;
            this.ErrorHandler = ex => Log.Error("Exception in Rabbit MQ Server: " + ex.Message, ex);
            RetryCount = DefaultRetryCount;
            AutoReconnect = true;
        }

        /// <summary>
        /// Register DTOs and handlers the MQ Server will process
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processMessageFn"></param>
        public virtual void RegisterHandler<T>(Func<IMessage<T>, object> processMessageFn)
        {
            RegisterHandler(processMessageFn, null, noOfThreads: 1);
        }
        /// <summary>
        /// Register DTOs and handlers the MQ Server will process using specified number of threads
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processMessageFn"></param>
        /// <param name="noOfThreads"></param>
        public virtual void RegisterHandler<T>(Func<IMessage<T>, object> processMessageFn, int noOfThreads)
        {
            RegisterHandler(processMessageFn, null, noOfThreads);
        }
        /// <summary>
        /// Register DTOs and handlers the MQ Server will process
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processMessageFn"></param>
        /// <param name="processExceptionEx"></param>
        public virtual void RegisterHandler<T>(Func<IMessage<T>, object> processMessageFn, Action<IMessageHandler, IMessage<T>, Exception> processExceptionEx)
        {
            RegisterHandler(processMessageFn, processExceptionEx, noOfThreads: 1);
        }

        /// <summary>
        /// Register DTOs and handlers the MQ Server will process using specified number of threads
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processMessageFn"></param>
        /// <param name="processExceptionEx"></param>
        /// <param name="noOfThreads"></param>
        /// <exception cref="System.ArgumentException">Message handler has already been registered for type: " + typeof(T).Name</exception>
        public virtual void RegisterHandler<T>(Func<IMessage<T>, object> processMessageFn, Action<IMessageHandler, IMessage<T>, Exception> processExceptionEx, int noOfThreads)
        {
            if (handlerMap.ContainsKey(typeof(T)))
                throw new ArgumentException("Message handler has already been registered for type: " + typeof(T).Name);

            handlerMap[typeof(T)] = CreateMessageHandlerFactory(processMessageFn, processExceptionEx);
            handlerThreadCountMap[typeof(T)] = noOfThreads;
        }

        /// <summary>
        /// Creates the message handler factory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processMessageFn">The process message function.</param>
        /// <param name="processExceptionEx">The process exception ex.</param>
        /// <returns></returns>
        protected IMessageHandlerFactory CreateMessageHandlerFactory<T>(Func<IMessage<T>, object> processMessageFn, Action<IMessageHandler, IMessage<T>, Exception> processExceptionEx)
        {
            return new MessageHandlerFactory<T>(this, processMessageFn, processExceptionEx)
            {
                RequestFilter = this.RequestFilter,
                ResponseFilter = this.ResponseFilter,
                PublishResponsesWhitelist = PublishResponsesWhitelist,    
                PublishToOutqWhitelist = PublishToOutqWhitelist,
                RetryCount = RetryCount,
            };
        }

        //Stats        
        /// <summary>
        /// The times started
        /// </summary>
        private long timesStarted = 0;
        /// <summary>
        /// The no of errors
        /// </summary>
        private long noOfErrors = 0;
        /// <summary>
        /// The no of continuous errors
        /// </summary>
        private int noOfContinuousErrors = 0;
        /// <summary>
        /// The last ex MSG
        /// </summary>
        private string lastExMsg = null;
        /// <summary>
        /// The status
        /// </summary>
        private int status;
        /// <summary>
        /// The MSG lock
        /// </summary>
        readonly object msgLock = new object();
        /// <summary>
        /// The do operation
        /// </summary>
        private long doOperation = WorkerOperation.NoOp;

        /// <summary>
        /// The bg thread
        /// </summary>
        private Thread bgThread; //Subscription controller thread        
        /// <summary>
        /// The bg thread count
        /// </summary>
        private long bgThreadCount = 0;
        /// <summary>
        /// Gets the bg thread count.
        /// </summary>
        /// <value>
        /// The bg thread count.
        /// </value>
        public long BgThreadCount => Interlocked.CompareExchange(ref bgThreadCount, 0, 0);

        /// <summary>
        /// Get Total Current Stats for all Message Handlers
        /// </summary>
        /// <returns></returns>
        public virtual IMessageHandlerStats GetStats()
        {
            lock (workers)
            {
                var total = new MessageHandlerStats("All Handlers");
                workers.ToList().ForEach(x => total.Add(x.GetStats()));
                return total;
            }
        }

        /// <summary>
        /// Get the status of the service. Potential Statuses: Disposed, Stopped, Stopping, Starting, Started
        /// </summary>
        /// <returns></returns>
        public virtual string GetStatus()
        {
            return WorkerStatus.ToString(Interlocked.CompareExchange(ref status, 0, 0));
        }
        /// <summary>
        /// Get a Stats dump
        /// </summary>
        /// <returns></returns>
        public virtual string GetStatsDescription()
        {
            lock (workers)
            {
                var sb = StringBuilderCache.Allocate().Append("#MQ SERVER STATS:\n");
                sb.AppendLine("===============");
                sb.AppendLine("Current Status: " + GetStatus());
                sb.AppendLine("Listening On: " + string.Join(", ", workers.ToList().ConvertAll(x => x.QueueName).ToArray()));
                sb.AppendLine("Times Started: " + Interlocked.CompareExchange(ref timesStarted, 0, 0));
                sb.AppendLine("Num of Errors: " + Interlocked.CompareExchange(ref noOfErrors, 0, 0));
                sb.AppendLine("Num of Continuous Errors: " + Interlocked.CompareExchange(ref noOfContinuousErrors, 0, 0));
                sb.AppendLine("Last ErrorMsg: " + lastExMsg);
                sb.AppendLine("===============");
                foreach (var worker in workers)
                {
                    sb.AppendLine(worker.GetStats().ToString());
                    sb.AppendLine("---------------\n");
                }
                return StringBuilderCache.ReturnAndFree(sb);
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Init()
        {
            if (workers != null) return;

            var workerBuilder = new List<RabbitMqWorker>();

            using (var channel = Connection.OpenChannel())
            {
                foreach (var entry in handlerMap)
                {
                    var msgType = entry.Key;
                    var handlerFactory = entry.Value;

                    var queueNames = new QueueNames(msgType);
                    var noOfThreads = handlerThreadCountMap[msgType];

                    if (PriorityQueuesWhitelist == null
                        || PriorityQueuesWhitelist.Any(x => x == msgType.Name))
                    {
                        noOfThreads.Times(i =>
                            workerBuilder.Add(new RabbitMqWorker(
                                messageFactory,
                                handlerFactory.CreateMessageHandler(),
                                queueNames.Priority,
                                WorkerErrorHandler,
                                AutoReconnect)));

                    }

                    noOfThreads.Times(i =>
                        workerBuilder.Add(new RabbitMqWorker(
                                messageFactory,
                                handlerFactory.CreateMessageHandler(),
                                queueNames.In,
                                WorkerErrorHandler,
                                AutoReconnect)));

                    channel.RegisterQueues(queueNames);
                }

                workers = workerBuilder.ToArray();

                queueWorkerIndexMap = new Dictionary<string, int[]>();
                for (var i = 0; i < workers.Length; i++)
                {
                    var worker = workers[i];

                    if (!queueWorkerIndexMap.TryGetValue(worker.QueueName, out var workerIds))
                    {
                        queueWorkerIndexMap[worker.QueueName] = new[] { i };
                    }
                    else
                    {
                        workerIds = new List<int>(workerIds) { i }.ToArray();
                        queueWorkerIndexMap[worker.QueueName] = workerIds;
                    }
                }
            }
        }

        /// <summary>
        /// Start the MQ Host if not already started.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">MQ Host has been disposed</exception>
        public virtual void Start()
        {
            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Started)
            {
                //Start any stopped worker threads
                StartWorkerThreads();
                return;
            }

            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Disposed)
                throw new ObjectDisposedException("MQ Host has been disposed");

            //Only 1 thread allowed past
            if (Interlocked.CompareExchange(ref status, WorkerStatus.Starting, WorkerStatus.Stopped) == WorkerStatus.Stopped)
            {
                //Should only be 1 thread past this point
                try
                {
                    Init();

                    if (workers == null || workers.Length == 0)
                    {
                        Log.Warn("Cannot start a MQ Server with no Message Handlers registered, ignoring.");
                        Interlocked.CompareExchange(ref status, WorkerStatus.Stopped, WorkerStatus.Starting);
                        return;
                    }

                    StartWorkerThreads();

                    //Don't kill us if we're the thread that's retrying to Start() after a failure.
                    if (bgThread != Thread.CurrentThread)
                    {
                        KillBgThreadIfExists();

                        bgThread = new Thread(RunLoop)
                        {
                            IsBackground = true,
                            Name = "Rabbit MQ Server " + Interlocked.Increment(ref bgThreadCount)
                        };
                        bgThread.Start();
                        Log.Debug("Started Background Thread: " + bgThread.Name);
                    }
                    else
                    {
                        Log.Debug("Retrying RunLoop() on Thread: " + bgThread.Name);
                        RunLoop();
                    }
                }
                catch (Exception ex)
                {
                    if (this.ErrorHandler != null)
                        this.ErrorHandler(ex);
                    else
                        throw;
                }
            }
        }

        /// <summary>
        /// Runs the loop.
        /// </summary>
        private void RunLoop()
        {
            if (Interlocked.CompareExchange(ref status, WorkerStatus.Started, WorkerStatus.Starting) != WorkerStatus.Starting) return;
            Interlocked.Increment(ref timesStarted);

            try
            {
                lock (msgLock)
                {
                    //RESET
                    while (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Started)
                    {
                        Monitor.Wait(msgLock);
                        Log.Debug("msgLock received...");

                        var op = Interlocked.CompareExchange(ref doOperation, WorkerOperation.NoOp, doOperation);
                        switch (op)
                        {
                            case WorkerOperation.Stop:
                                Log.Debug("Stop Command Issued");

                                Interlocked.CompareExchange(ref status, WorkerStatus.Stopping, WorkerStatus.Started);
                                try
                                {
                                    StopWorkerThreads();
                                }
                                finally
                                {
                                    Interlocked.CompareExchange(ref status, WorkerStatus.Stopped, WorkerStatus.Stopping);
                                }
                                return; //exits

                            case WorkerOperation.Restart:
                                Log.Debug("Restart Command Issued");

                                Interlocked.CompareExchange(ref status, WorkerStatus.Stopping, WorkerStatus.Started);
                                try
                                {
                                    StopWorkerThreads();
                                }
                                finally
                                {
                                    Interlocked.CompareExchange(ref status, WorkerStatus.Stopped, WorkerStatus.Stopping);
                                }

                                StartWorkerThreads();
                                Interlocked.CompareExchange(ref status, WorkerStatus.Started, WorkerStatus.Stopped);
                                break; //continues
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastExMsg = ex.Message;
                Interlocked.Increment(ref noOfErrors);
                Interlocked.Increment(ref noOfContinuousErrors);

                if (Interlocked.CompareExchange(ref status, WorkerStatus.Stopped, WorkerStatus.Started) != WorkerStatus.Started)
                    Interlocked.CompareExchange(ref status, WorkerStatus.Stopped, WorkerStatus.Stopping);

                StopWorkerThreads();

                if (this.ErrorHandler != null)
                    this.ErrorHandler(ex);

                if (KeepAliveRetryAfterMs != null)
                {
                    Thread.Sleep(KeepAliveRetryAfterMs.Value);
                    Start();
                }
            }
            Log.Debug("Exiting RunLoop()...");
        }

        /// <summary>
        /// Stop the MQ Host if not already stopped.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">MQ Host has been disposed</exception>
        public virtual void Stop()
        {
            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Disposed)
                throw new ObjectDisposedException("MQ Host has been disposed");

            if (Interlocked.CompareExchange(ref status, WorkerStatus.Stopping, WorkerStatus.Started) == WorkerStatus.Started)
            {
                lock (msgLock)
                {
                    Interlocked.CompareExchange(ref doOperation, WorkerOperation.Stop, doOperation);
                    Monitor.Pulse(msgLock);
                }
            }
        }
        /// <summary>
        /// Waits for workers to stop.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public virtual void WaitForWorkersToStop(TimeSpan? timeout=null)
        {
            ExecUtils.RetryUntilTrue(
                () => Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Stopped,
                timeout);            
        }

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">MQ Host has been disposed</exception>
        public virtual void Restart()
        {
            if (Interlocked.CompareExchange(ref status, 0, 0) == WorkerStatus.Disposed)
                throw new ObjectDisposedException("MQ Host has been disposed");

            if (Interlocked.CompareExchange(ref status, WorkerStatus.Stopping, WorkerStatus.Started) == WorkerStatus.Started)
            {
                lock (msgLock)
                {
                    Interlocked.CompareExchange(ref doOperation, WorkerOperation.Restart, doOperation);
                    Monitor.Pulse(msgLock);
                }
            }
        }

        /// <summary>
        /// Starts the worker threads.
        /// </summary>
        public virtual void StartWorkerThreads()
        {
            if (Log.IsDebugEnabled)
                Log.Debug("Starting all Rabbit MQ Server worker threads...");

            foreach (var worker in workers)
            {
                try
                {
                    worker.Start();
                }
                catch (Exception ex)
                {
                    ErrorHandler?.Invoke(ex);
                    Log.Warn("Could not START Rabbit MQ worker thread: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Stops the worker threads.
        /// </summary>
        public virtual void StopWorkerThreads()
        {
            if (Log.IsDebugEnabled)
                Log.Debug("Stopping all Rabbit MQ Server worker threads...");

            foreach (var worker in workers)
            {
                try
                {
                    worker.Stop();
                }
                catch (Exception ex)
                {
                    ErrorHandler?.Invoke(ex);
                    Log.Warn("Could not STOP Rabbit MQ worker thread: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Disposes the worker threads.
        /// </summary>
        void DisposeWorkerThreads()
        {
            if (Log.IsDebugEnabled)
                Log.Debug("Disposing all Rabbit MQ Server worker threads...");
            workers?.Each(x => x.Dispose());
        }
        /// <summary>
        /// Workers the error handler.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="ex">The ex.</param>
        void WorkerErrorHandler(RabbitMqWorker source, Exception ex)
        {
            Log.Error("Received exception in Worker: " + source.QueueName, ex);
            for (int i = 0; i < workers.Length; i++)
            {
                var worker = workers[i];
                if (worker == source)
                {
                    Log.Debug($"Starting new {source.QueueName} Worker at index {i}...");
                    workers[i] = source.Clone();
                    workers[i].Start();
                    worker.Dispose();
                    return;
                }
            }
        }

        /// <summary>
        /// Kills the bg thread if exists.
        /// </summary>
        private void KillBgThreadIfExists()
        {
            if (bgThread != null && bgThread.IsAlive)
            {
                //give it a small chance to die gracefully
                if (!bgThread.Join(500))
                {
                    //Ideally we shouldn't get here, but lets try our hardest to clean it up
                    Log.Warn("Interrupting previous Background Thread: " + bgThread.Name);
                    bgThread.Interrupt();
                    if (!bgThread.Join(TimeSpan.FromSeconds(3)))
                    {
                        Log.Warn(bgThread.Name + " just wont die, so we're now aborting it...");
                        bgThread.Abort();
                    }
                }
                bgThread = null;
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
                DisposeWorkerThreads();
            }
            catch (Exception ex)
            {
                Log.Error("Error DisposeWorkerThreads(): ", ex);
            }

            try
            {
                Thread.Sleep(100); //give it a small chance to die gracefully
                KillBgThreadIfExists();
            }
            catch (Exception ex)
            {
                ErrorHandler?.Invoke(ex);
            }

            try
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection = null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler?.Invoke(ex);
            }

        }
    }
}
