using NoobCore.Messaging;
using NoobCore.RabbitMq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class HelloRabbit
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    //[TestFixture, Ignore("Integration Test")]
    [TestFixture]
    public class RabbitMqTests
    {
        /// <summary>
        /// The mq factory
        /// </summary>
        private readonly ConnectionFactory mqFactory = new ConnectionFactory
        {
            HostName = TestsConfig.RabbitMqHost
        };
        /// <summary>
        /// The exchange
        /// </summary>
        private const string Exchange = "mq:tests";
        /// <summary>
        /// The exchange DLQ
        /// </summary>
        private const string ExchangeDlq = "mq:tests.dlq";
        /// <summary>
        /// The exchange topic
        /// </summary>
        private const string ExchangeTopic = "mq:tests.topic";
        /// <summary>
        /// The exchange fanout
        /// </summary>
        private const string ExchangeFanout = "mq:tests.fanout";


        /// <summary>
        /// Tests the fixture set up.
        /// </summary>
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.RegisterDirectExchange(Exchange);
                channel.RegisterDlqExchange(ExchangeDlq);
                channel.RegisterTopicExchange(ExchangeTopic);

                RegisterQueue(channel, QueueNames<HelloRabbit>.In);
                RegisterQueue(channel, QueueNames<HelloRabbit>.Priority);
                RegisterDlq(channel, QueueNames<HelloRabbit>.Dlq);
                RegisterTopic(channel, QueueNames<HelloRabbit>.Out);
                RegisterQueue(channel, QueueNames<HelloRabbit>.In, exchange: ExchangeTopic);

                channel.PurgeQueue<HelloRabbit>();
            }
        }


        /// <summary>
        /// Registers the queue.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="exchange">The exchange.</param>
        public static void RegisterQueue(IModel channel, string queueName, string exchange = Exchange)
        {
            var args = new Dictionary<string, object> {
                {"x-dead-letter-exchange", ExchangeDlq },
                {"x-dead-letter-routing-key", queueName.Replace(".inq",".dlq").Replace(".priorityq",".dlq") },
            };
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);
            channel.QueueBind(queueName, exchange, routingKey: queueName);
        }

        /// <summary>
        /// Registers the topic.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queueName">Name of the queue.</param>
        public static void RegisterTopic(IModel channel, string queueName)
        {
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queueName, ExchangeTopic, routingKey: queueName);
        }

        /// <summary>
        /// Registers the DLQ.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queueName">Name of the queue.</param>
        public static void RegisterDlq(IModel channel, string queueName)
        {
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queueName, ExchangeDlq, routingKey: queueName);
        }

        /// <summary>
        /// Exchanges the delete.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="exchange">The exchange.</param>
        public void ExchangeDelete(IModel channel, string exchange)
        {
            try
            {
                channel.ExchangeDelete(exchange);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ExchangeDelete(): {ex.Message}");
            }
        }

        /// <summary>
        /// Determines whether this instance [can publish messages to rabbit mq].
        /// </summary>
        [Test]
        public void Can_publish_messages_to_RabbitMQ()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                5.Times(i =>
                {
                    byte[] payload = new HelloRabbit { Name = $"World! #{i}" }.ToJson().ToUtf8Bytes();
                    var props = channel.CreateBasicProperties();
                    props.Persistent = true;

                    channel.BasicPublish(exchange: Exchange,
                        routingKey: QueueNames<HelloRabbit>.In, basicProperties: props, body: payload);

                    Console.WriteLine("Sent Message " + i);
                    Thread.Sleep(1000);
                });
            }
        }

        /// <summary>
        /// Determines whether this instance [can consume messages from rabbit mq with basic get].
        /// </summary>
        [Test]
        public void Can_consume_messages_from_RabbitMQ_with_BasicGet()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                PublishHelloRabbit(channel);

                while (true)
                {
                    var basicGetMsg = channel.BasicGet(QueueNames<HelloRabbit>.In, autoAck: false);

                    if (basicGetMsg == null)
                    {
                        Console.WriteLine("End of the road...");
                        return;
                    }

                    var msg = basicGetMsg.Body.ToArray().FromUtf8Bytes().FromJson<HelloRabbit>();

                    Thread.Sleep(1000);

                    channel.BasicAck(basicGetMsg.DeliveryTag, multiple: false);
                }
            }
        }


        /// <summary>
        /// Publishes the hello rabbit.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="text">The text.</param>
        private static void PublishHelloRabbit(IModel channel, string text = "World!")
        {
            byte[] payload = new HelloRabbit { Name = text }.ToJson().ToUtf8Bytes();
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            channel.BasicPublish(Exchange, QueueNames<HelloRabbit>.In, props, payload);
        }


        /// <summary>
        /// Determines whether this instance [can consume messages from rabbit mq with basic consume].
        /// </summary>
        [Test]
        public void Can_consume_messages_from_RabbitMQ_with_BasicConsume()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Thread.Sleep(100);
                    PublishHelloRabbit(channel);
                });
                // Set up a synchronization barrier.
                using (var barrier = new ManualResetEventSlim(false))
                {
                    var consumer = new EventingBasicConsumer(channel);
                    var consumerTag = channel.BasicConsume(QueueNames<HelloRabbit>.In, autoAck: false, consumer: consumer);
                    string? recvMsg = null;
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            Console.WriteLine("Dequeued");
                            var body = ea.Body.ToArray();
                            recvMsg = Encoding.UTF8.GetString(body);

                            channel.BasicAck(ea.DeliveryTag, multiple: false);
                            Console.WriteLine($" [x] Received {recvMsg}");

                            Assert.That(recvMsg, Is.Not.Null);
                        }
                        catch (OperationInterruptedException)
                        {
                            // The consumer was removed, either through
                            // channel or connection closure, or through the
                            // action of IModel.BasicCancel().
                            Console.WriteLine("End of the road...");
                        }
                        finally
                        {
                            barrier.Set(); // Signal Event fired.
                        }
                    };
                    barrier.Wait(); // Wait for Event to fire.
                }
              



            }
        }

    }
}
