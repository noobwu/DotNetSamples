using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Util;

namespace NoobCore.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="RabbitMQ.Client.DefaultBasicConsumer" />
    public class RabbitMqBasicConsumer : EventingBasicConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqBasicConsumer"/> class.
        /// </summary>
        /// <param name="model">Common AMQP model.</param>
        public RabbitMqBasicConsumer(IModel model): base(model) { }
    }
}