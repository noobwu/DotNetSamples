using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using NoobCore.Logging;
using NoobCore.Messaging;
using RabbitMQ.Client.Events;

namespace NoobCore.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.Messaging.IMessageProducer" />
    /// <seealso cref="NoobCore.IOneWayClient" />
    public class RabbitMqProducer : IMessageProducer, IOneWayClient
    {
        /// <summary>
        /// The log
        /// </summary>
        public static ILog Log = LogManager.GetLogger(typeof(RabbitMqProducer));
        /// <summary>
        /// The MSG factory
        /// </summary>
        protected readonly RabbitMqMessageFactory msgFactory;
        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>
        /// The retry count.
        /// </value>
        public int RetryCount { get; set; }
        /// <summary>
        /// Gets or sets the on published callback.
        /// </summary>
        /// <value>
        /// The on published callback.
        /// </value>
        public Action OnPublishedCallback { get; set; }
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
        //http://www.rabbitmq.com/blog/2012/04/25/rabbitmq-performance-measurements-part-2/
        //http://www.rabbitmq.com/amqp-0-9-1-reference.html
        public ushort PrefetchCount { get; set; } = 20;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqProducer"/> class.
        /// </summary>
        /// <param name="msgFactory">The MSG factory.</param>
        public RabbitMqProducer(RabbitMqMessageFactory msgFactory)
        {
            this.msgFactory = msgFactory;
        }
        /// <summary>
        /// Publishes the specified message body.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageBody">The message body.</param>
        public virtual void Publish<T>(T messageBody)
        {
            if (messageBody is IMessage message)
            {
                Publish(message.ToInQueueName(), message);
            }
            else
            {
                Publish(new Message<T>(messageBody));
            }
        }
        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        public virtual void Publish<T>(IMessage<T> message)
        {
            Publish(message.ToInQueueName(), message);
        }
        /// <summary>
        /// Publishes the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        public virtual void Publish(string queueName, IMessage message)
        {
            Publish(queueName, message, QueueNames.Exchange);
        }
        /// <summary>
        /// Sends the one way.
        /// </summary>
        /// <param name="requestDto">The request dto.</param>
        public virtual void SendOneWay(object requestDto)
        {
            Publish(MessageFactory.Create(requestDto));
        }
        /// <summary>
        /// Sends the one way.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="requestDto">The request dto.</param>
        public virtual void SendOneWay(string queueName, object requestDto)
        {
            Publish(queueName, MessageFactory.Create(requestDto));
        }
        /// <summary>
        /// Sends all one way.
        /// </summary>
        /// <param name="requests">The requests.</param>
        public virtual void SendAllOneWay(IEnumerable<object> requests)
        {
            if (requests == null) return;
            foreach (var request in requests)
            {
                SendOneWay(request);
            }
        }
        /// <summary>
        /// Publishes the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <param name="exchange">The exchange.</param>
        public virtual void Publish(string queueName, IMessage message, string exchange)
        {
            var props = Channel.CreateBasicProperties();
            props.Persistent = true;
            props.PopulateFromMessage(message);

            if (message.Meta != null)
            {
                props.Headers = new Dictionary<string, object>();
                foreach (var entry in message.Meta)
                {
                    props.Headers[entry.Key] = entry.Value;
                }
            }

            PublishMessageFilter?.Invoke(queueName, props, message);

            var messageBytes = message.Body.ToJson().ToUtf8Bytes();

            PublishMessage(exchange ?? QueueNames.Exchange,
                routingKey: queueName,
                basicProperties: props, body: messageBytes);

            OnPublishedCallback?.Invoke();
        }

        /// <summary>
        /// The queues
        /// </summary>
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
        /// Gets the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="noAck">if set to <c>true</c> [no ack].</param>
        /// <returns></returns>
        public virtual BasicGetResult GetMessage(string queueName, bool noAck)
        {
            try
            {
                if (!Queues.Contains(queueName))
                {
                    Channel.RegisterQueueByName(queueName);
                    Queues = new HashSet<string>(Queues) { queueName };
                }

                var basicMsg = Channel.BasicGet(queueName, autoAck: noAck);

                GetMessageFilter?.Invoke(queueName, basicMsg);

                return basicMsg;
            }
            catch (OperationInterruptedException ex)
            {
                if (ex.Is404())
                {
                    Channel.RegisterQueueByName(queueName);

                    return Channel.BasicGet(queueName, autoAck: noAck);
                }
                throw;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (channel != null)
            {
                try
                {
                    channel.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error("Error trying to dispose RabbitMqProducer model", ex);
                }
                channel = null;
            }
            if (connection != null)
            {
                try
                {
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error("Error trying to dispose RabbitMqProducer connection", ex);
                }
                connection = null;
            }
        }
    }
}