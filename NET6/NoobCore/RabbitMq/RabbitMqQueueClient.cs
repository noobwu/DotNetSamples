using System;
using System.Threading;
using RabbitMQ.Client;
using NoobCore.Messaging;

namespace NoobCore.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.RabbitMq.RabbitMqProducer" />
    /// <seealso cref="NoobCore.Messaging.IMessageQueueClient" />
    public class RabbitMqQueueClient : RabbitMqProducer, IMessageQueueClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqQueueClient"/> class.
        /// </summary>
        /// <param name="msgFactory">The MSG factory.</param>
        public RabbitMqQueueClient(RabbitMqMessageFactory msgFactory)
            : base(msgFactory) {}

        /// <summary>
        /// Publish the specified message into the transient queue @queueName
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        public virtual void Notify(string queueName, IMessage message)
        {
            var json = message.Body.ToJson();
            var messageBytes = json.ToUtf8Bytes();

            PublishMessage(QueueNames.ExchangeTopic,
                routingKey: queueName,
                basicProperties: null, body: messageBytes);
        }
        /// <summary>
        /// Synchronous blocking get.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public virtual IMessage<T> Get<T>(string queueName, TimeSpan? timeOut = null)
        {
            var now = DateTime.UtcNow;

            while (timeOut == null || (DateTime.UtcNow - now) < timeOut.Value)
            {
                var basicMsg = GetMessage(queueName, noAck: false);
                if (basicMsg != null)
                {
                    return basicMsg.ToMessage<T>();
                }
                Thread.Sleep(100);
            }

            return null;
        }

        public virtual IMessage<T> GetAsync<T>(string queueName)
        {
            var basicMsg = GetMessage(queueName, noAck: false);
            return basicMsg.ToMessage<T>();
        }

        public virtual void Ack(IMessage message)
        {
            var deliveryTag = ulong.Parse(message.Tag);
            Channel.BasicAck(deliveryTag, multiple:false);
        }

        public virtual void Nak(IMessage message, bool requeue, Exception exception = null)
        {
            try
            {
                if (requeue)
                {
                    var deliveryTag = ulong.Parse(message.Tag);
                    Channel.BasicNack(deliveryTag, multiple: false, requeue: requeue);
                }
                else
                {
                    Publish(message.ToDlqQueueName(), message, QueueNames.ExchangeDlq);
                    Ack(message);
                }
            }
            catch (Exception)
            {
                var deliveryTag = ulong.Parse(message.Tag);
                Channel.BasicNack(deliveryTag, multiple: false, requeue: requeue);
            }
        }

        public virtual IMessage<T> CreateMessage<T>(object mqResponse)
        {
            if (mqResponse is BasicGetResult msgResult)
            {
                return msgResult.ToMessage<T>();
            }

            return (IMessage<T>)mqResponse;
        }

        public virtual string GetTempQueueName()
        {
            var anonMq = Channel.QueueDeclare(
                queue: QueueNames.GetTempQueueName(),
                durable:false,
                exclusive:true,
                autoDelete:true,
                arguments:null);

            return anonMq.QueueName;
        }
    }
}