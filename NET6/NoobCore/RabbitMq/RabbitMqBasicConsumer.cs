﻿using RabbitMQ.Client;
using RabbitMQ.Util;

namespace NoobCore.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="RabbitMQ.Client.DefaultBasicConsumer" />
    public class RabbitMqBasicConsumer : DefaultBasicConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqBasicConsumer"/> class.
        /// </summary>
        /// <param name="model">Common AMQP model.</param>
        public RabbitMqBasicConsumer(IModel model): base(model) { }
    }
}