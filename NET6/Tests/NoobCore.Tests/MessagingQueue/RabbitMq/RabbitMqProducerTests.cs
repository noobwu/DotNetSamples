using NoobCore.Interfaces;
using NoobCore.Messaging;
using NoobCore.RabbitMq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace NoobCore.Tests.MessagingQueue.RabbitMq
{


    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class RabbitMqProducerTests
    {
        /// <summary>
        /// 
        /// </summary>
        public RabbitMqProducerTests() {
            msgFactory=new RabbitMqMessageFactory();
        }
        /// <summary>
        /// 
        /// </summary>
        [TestCase]
        public void PublishNormalMsg() {
            using var channel = connection.CreateModel();
            var request = new HelloIntro { Name = "World" };
            var requestInq = MessageFactory.Create(request).ToInQueueName();
            Assert.That(requestInq, Is.EqualTo("site1.mq:HelloIntro.inq"));
            channel.QueueDeclare(queue: requestInq, durable: false, exclusive: false, autoDelete: false, arguments: null);

            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: QueueNames.Exchange, routingKey: "hello", basicProperties: null, body: body);
            Console.WriteLine($" [x] Sent {message}");
        }

        static HashSet<string> Queues = new HashSet<string>();
        /// <summary>
        /// Publishes the message.
        /// </summary>
        /// <param name="exchange">The exchange.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="basicProperties">The basic properties.</param>
        /// <param name="body">The body.</param>
        public virtual void PublishMessage(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body)
        {
            try
            {
                // In case of server named queues (client declared queue with channel.declare()), assume queue already exists
                //(redeclaration would result in error anyway since queue was marked as exclusive) and publish to default exchange
                if (routingKey.IsServerNamedQueue())
                {
                    Channel.BasicPublish("", routingKey, basicProperties, body);
}
                else
                {
                    if (!Queues.Contains(routingKey))
{
Channel.RegisterQueueByName(routingKey);
                        Queues = new HashSet<string>(Queues) { routingKey };
                    }

                    Channel.BasicPublish(exchange, routingKey, basicProperties, body);
                }

            }
            catch (OperationInterruptedException ex)
            {
                if (ex.Is404())
                {
                    // In case of server named queues (client declared queue with channel.declare()), assume queue already exists (redeclaration would result in error anyway since queue was marked as exclusive) and publish to default exchange
                    if (routingKey.IsServerNamedQueue())
                    {
                        Channel.BasicPublish("", routingKey, basicProperties, body);
                    }
                    else
                    {
                        Channel.RegisterExchangeByName(exchange);

                        Channel.BasicPublish(exchange, routingKey, basicProperties, body);
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected readonly RabbitMqMessageFactory msgFactory;
        /// <summary>
        /// 
        /// </summary>
        private IConnection connection;
        /// <summary>
        /// 
        /// </summary>
        public IConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = msgFactory.ConnectionFactory.CreateConnection();
                }
                return connection;
            }
        }
        //http://www.rabbitmq.com/blog/2012/04/25/rabbitmq-performance-measurements-part-2/
        //http://www.rabbitmq.com/amqp-0-9-1-reference.html        
        /// <summary>
        /// Gets or sets the prefetch count.
        /// </summary>
        /// <value>
        /// The prefetch count.
        /// </value>
        public ushort PrefetchCount { get; set; } = 20;
        /// <summary>
        /// The channel
        /// </summary>
        private IModel channel;
        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public IModel Channel
        {
            get
            {
                if (channel == null || !channel.IsOpen)
                {
                    channel = Connection.OpenChannel();
                    //prefetch size is no supported by RabbitMQ
                    //http://www.rabbitmq.com/specification.html#method-status-basic.qos
                    channel.BasicQos(prefetchSize: 0, prefetchCount: PrefetchCount, global: false);
                }
                return channel;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RabbitMqMessageFactory : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public ConnectionFactory ConnectionFactory { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        private int retryCount;
        /// <summary>
        /// 
        /// </summary>
        public int RetryCount
        {
            get => retryCount;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(RetryCount),
                        "Rabbit MQ RetryCount must be 0-1");
                }

                retryCount = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [use polling].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use polling]; otherwise, <c>false</c>.
        /// </value>
        public bool UsePolling { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqMessageFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.ArgumentNullException">connectionString</exception>
        public RabbitMqMessageFactory(string connectionString = "localhost",
            string? username = null, string? password = null)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            ConnectionFactory = new ConnectionFactory
            {
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
            };

            if (username != null)
            {
                ConnectionFactory.UserName = username;
            }
            if (password != null)
                ConnectionFactory.Password = password;

            if (connectionString.StartsWith("amqp://") || connectionString.StartsWith("amqps://"))
            {
                ConnectionFactory.Uri = new Uri(connectionString);
            }
            else
            {
                var parts = connectionString.SplitOnFirst(':');
                var hostName = parts[0];
                ConnectionFactory.HostName = hostName;

                if (parts.Length > 1)
                {
                    ConnectionFactory.Port = parts[1].ToInt();
                }
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqMessageFactory"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        public RabbitMqMessageFactory(ConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HelloIntro : IReturn<HelloIntroResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class HelloIntroResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string Result { get; set; }
    }
}
