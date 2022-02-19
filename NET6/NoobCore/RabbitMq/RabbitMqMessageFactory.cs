using System;
using RabbitMQ.Client;
using NoobCore.Messaging;

namespace NoobCore.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.Messaging.IMessageFactory" />
    public class RabbitMqMessageFactory : IMessageFactory
    {
        /// <summary>
        /// Gets the connection factory.
        /// </summary>
        /// <value>
        /// The connection factory.
        /// </value>
        public ConnectionFactory ConnectionFactory { get; private set; }
        /// <summary>
        /// Gets or sets the mq queue client filter.
        /// </summary>
        /// <value>
        /// The mq queue client filter.
        /// </value>
        public Action<RabbitMqQueueClient> MqQueueClientFilter { get; set; }
        /// <summary>
        /// Gets or sets the mq producer filter.
        /// </summary>
        /// <value>
        /// The mq producer filter.
        /// </value>
        public Action<RabbitMqProducer> MqProducerFilter { get; set; }
        /// <summary>
        /// Gets or sets the publish message filter.
        /// </summary>
        /// <value>
        /// The publish message filter.
        /// </value>
        public Action<string, IBasicProperties, IMessage> PublishMessageFilter { get; set; }
        /// <summary>
        /// Gets or sets the get message filter.
        /// </summary>
        /// <value>
        /// The get message filter.
        /// </value>
        public Action<string, BasicGetResult> GetMessageFilter { get; set; }
        /// <summary>
        /// The retry count
        /// </summary>
        private int retryCount;
        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>
        /// The retry count.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">RetryCount - Rabbit MQ RetryCount must be 0-1</exception>
        public int RetryCount
        {
            get => retryCount;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(RetryCount), 
                        "Rabbit MQ RetryCount must be 0-1");

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
            string username = null, string password = null)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            ConnectionFactory = new ConnectionFactory {
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
            };

            if (username != null)
                ConnectionFactory.UserName = username;
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
        /// Creates the message queue client.
        /// </summary>
        /// <returns></returns>
        public virtual IMessageQueueClient CreateMessageQueueClient()
        {
            var client = new RabbitMqQueueClient(this) {
                RetryCount = RetryCount,
                PublishMessageFilter = PublishMessageFilter,
                GetMessageFilter = GetMessageFilter,
            };
            MqQueueClientFilter?.Invoke(client);
            return client;
        }
        /// <summary>
        /// Creates the message producer.
        /// </summary>
        /// <returns></returns>
        public virtual IMessageProducer CreateMessageProducer()
        {
            var client = new RabbitMqProducer(this) {
                RetryCount = RetryCount,
                PublishMessageFilter = PublishMessageFilter,
                GetMessageFilter = GetMessageFilter,
            };
            MqProducerFilter?.Invoke(client);
            return client;
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}