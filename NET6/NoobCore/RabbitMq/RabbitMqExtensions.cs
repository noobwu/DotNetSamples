//Url：https://github.com/ServiceStack/ServiceStack/blob/main/ServiceStack/src/ServiceStack.RabbitMq/RabbitMqExtensions.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoobCore.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace NoobCore.RabbitMq
{
    public static class RabbitMqExtensions
    {
        /// <summary>
        /// Opens the channel.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public static IModel OpenChannel(this IConnection connection)
        {
            var channel = connection.CreateModel();
            channel.RegisterDirectExchange();
            channel.RegisterDlqExchange();
            channel.RegisterTopicExchange();
            return channel;
        }

        /// <summary>
        /// Registers the direct exchange.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        public static void RegisterDirectExchange(this IModel channel, string exchangeName = null)
        {
            channel.ExchangeDeclare(exchangeName ?? QueueNames.Exchange, "direct", durable: true, autoDelete: false, arguments: null);
        }

        /// <summary>
        /// Registers the DLQ exchange.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        public static void RegisterDlqExchange(this IModel channel, string exchangeName = null)
        {
            channel.ExchangeDeclare(exchangeName ?? QueueNames.ExchangeDlq, "direct", durable: true, autoDelete: false, arguments: null);
        }

        /// <summary>
        /// Registers the topic exchange.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        public static void RegisterTopicExchange(this IModel channel, string exchangeName = null)
        {
            channel.ExchangeDeclare(exchangeName ?? QueueNames.ExchangeTopic, "topic", durable: false, autoDelete: false, arguments: null);
        }

        /// <summary>
        /// Registers the fanout exchange.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        public static void RegisterFanoutExchange(this IModel channel, string exchangeName)
        {
            channel.ExchangeDeclare(exchangeName, "fanout", durable: false, autoDelete: false, arguments: null);
        }


        /// <summary>
        /// Registers the queues.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel">The channel.</param>
        public static void RegisterQueues<T>(this IModel channel)
        {
            channel.RegisterQueue(QueueNames<T>.In);
            channel.RegisterQueue(QueueNames<T>.Priority);
            channel.RegisterTopic(QueueNames<T>.Out);
            channel.RegisterDlq(QueueNames<T>.Dlq);
        }


        /// <summary>
        /// Registers the queues.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queueNames">The queue names.</param>
        public static void RegisterQueues(this IModel channel, QueueNames queueNames)
        {
            channel.RegisterQueue(queueNames.In);
            channel.RegisterQueue(queueNames.Priority);
            channel.RegisterTopic(queueNames.Out);
            channel.RegisterDlq(queueNames.Dlq);
        }

        /// <summary>
        /// Registers the queue.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queueName">Name of the queue.</param>
        public static void RegisterQueue(this IModel channel, string queueName)
        {
            var args = new Dictionary<string, object> {
                {"x-dead-letter-exchange", QueueNames.ExchangeDlq },
                {"x-dead-letter-routing-key", queueName.Replace(".inq",".dlq").Replace(".priorityq",".dlq") },
            };

            //GetRabbitMqServer()?.CreateQueueFilter?.Invoke(queueName, args);

            if (!QueueNames.IsTempQueue(queueName)) //Already declared in GetTempQueueName()
            {
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);
            }

            channel.QueueBind(queueName, QueueNames.Exchange, routingKey: queueName);
        }

        /// <summary>
        /// Registers the DLQ.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queueName">Name of the queue.</param>
        public static void RegisterDlq(this IModel channel, string queueName)
        {
            var args = new Dictionary<string, object>();

            //GetRabbitMqServer()?.CreateQueueFilter?.Invoke(queueName, args);

            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);
            channel.QueueBind(queueName, QueueNames.ExchangeDlq, routingKey: queueName);
        }

        /// <summary>
        /// Registers the topic.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queueName">Name of the queue.</param>
        public static void RegisterTopic(this IModel channel, string queueName)
        {
            var args = new Dictionary<string, object>();

            //GetRabbitMqServer()?.CreateTopicFilter?.Invoke(queueName, args);

            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: args);
            channel.QueueBind(queueName, QueueNames.ExchangeTopic, routingKey: queueName);
        }
        /// <summary>
        /// Registers the name of the exchange by.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="exchange">The exchange.</param>
        public static void RegisterExchangeByName(this IModel channel, string exchange)
        {
            if (exchange.EndsWith(".dlq"))
                channel.RegisterDlqExchange(exchange);
            else if (exchange.EndsWith(".topic"))
                channel.RegisterTopicExchange(exchange);
            else
                channel.RegisterDirectExchange(exchange);
        }
        /// <summary>
        /// Registers the name of the queue by.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queueName">Name of the queue.</param>
        public static void RegisterQueueByName(this IModel channel, string queueName)
        {
            if (queueName.EndsWith(".dlq"))
            {
                channel.RegisterDlq(queueName);
            }
            else if (queueName.EndsWith(".outq"))
            {
                channel.RegisterTopic(queueName);
            }
            else
            {
                channel.RegisterQueue(queueName);
            }
        }

        /// <summary>
        /// Is404s the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static bool Is404(this OperationInterruptedException ex)
        {
            return ex.Message.Contains("code=404");
        }
        /// <summary>
        /// Determines whether [is server named queue].
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>
        ///   <c>true</c> if [is server named queue] [the specified queue name]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">queueName</exception>
        public static bool IsServerNamedQueue(this string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException("queueName");
            }

            var lowerCaseQueue = queueName.ToLower();
            return lowerCaseQueue.StartsWith("amq.")
                || lowerCaseQueue.StartsWith(QueueNames.TempMqPrefix);
        }

        /// <summary>
        /// Populates from message.
        /// </summary>
        /// <param name="props">The props.</param>
        /// <param name="message">The message.</param>
        public static void PopulateFromMessage(this IBasicProperties props, IMessage message)
        {
            props.MessageId = message.Id.ToString();
            props.Timestamp = new AmqpTimestamp(message.CreatedDate.ToUnixTime());
            props.Priority = (byte)message.Priority;
            props.ContentType = MimeTypes.Json;

            if (message.Body != null)
            {
                props.Type = message.Body.GetType().Name;
            }

            if (message.ReplyTo != null)
            {
                props.ReplyTo = message.ReplyTo;
            }

            if (message.ReplyId != null)
            {
                props.CorrelationId = message.ReplyId.Value.ToString();
            }

            if (message.Error != null)
            {
                if (props.Headers == null)
                    props.Headers = new Dictionary<string, object>();
                props.Headers["Error"] = message.Error.ToJson();
            }
        }

        /// <summary>
        /// Converts to message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msgResult">The MSG result.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">Unknown Content-Type: " + props.ContentType</exception>
        public static IMessage<T> ToMessage<T>(this BasicGetResult msgResult)
        {
            if (msgResult == null)
                return null;

            var props = msgResult.BasicProperties;
            T body;

            if (string.IsNullOrEmpty(props.ContentType) || props.ContentType.MatchesContentType(MimeTypes.Json))
            {
                var json = msgResult.Body.ToArray().FromUtf8Bytes();
                body = json.FromJson<T>();
            }
            else
            {
                throw new NotSupportedException("Unknown Content-Type: " + props.ContentType);
            }
            var message = new Message<T>(body)
            {
                Id = props.MessageId != null ? Guid.Parse(props.MessageId) : new Guid(),
                CreatedDate = ((int)props.Timestamp.UnixTime).FromUnixTime(),
                Priority = props.Priority,
                ReplyTo = props.ReplyTo,
                Tag = msgResult.DeliveryTag.ToString(),
                RetryAttempts = msgResult.Redelivered ? 1 : 0,
            };

            if (props.CorrelationId != null)
            {
                message.ReplyId = Guid.Parse(props.CorrelationId);
            }

            if (props.Headers != null)
            {
                foreach (var entry in props.Headers)
                {
                    if (entry.Key == "Error")
                    {
                        var errors = entry.Value;
                        if (errors != null)
                        {
                            var errorsJson = errors is byte[] errorBytes
                                ? errorBytes.FromUtf8Bytes()
                                : errors.ToString();
                            message.Error = errorsJson.FromJson<ResponseStatus>();
                        }
                    }
                    else
                    {
                        if (message.Meta == null)
                            message.Meta = new Dictionary<string, string>();

                        var value = entry.Value is byte[] bytes
                            ? bytes.FromUtf8Bytes()
                            : entry.Value?.ToString();

                        message.Meta[entry.Key] = value;
                    }
                }
            }

            return message;
        }
    }
}
