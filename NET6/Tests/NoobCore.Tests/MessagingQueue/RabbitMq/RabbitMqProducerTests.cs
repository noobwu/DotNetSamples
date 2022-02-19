using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.MessagingQueue.RabbitMq
{
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

    public class RabbitMqProducerTests
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly RabbitMqMessageFactory msgFactory;
        private IConnection connection;
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
    }
}
