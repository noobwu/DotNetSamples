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
                //Url：https://stackoverflow.com/questions/71046474/unit-test-running-forever-for-call-back-method-in-c-sharp/71048247
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


        /// <summary>
        /// Publishings the message with routing key sends only to registered queue.
        /// </summary>
        [Test]
        public void Publishing_message_with_routingKey_sends_only_to_registered_queue()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                PublishHelloRabbit(channel);

                var basicGetMsg = channel.BasicGet(QueueNames<HelloRabbit>.In, autoAck: true);
                Assert.That(basicGetMsg, Is.Not.Null);

                basicGetMsg = channel.BasicGet(QueueNames<HelloRabbit>.Priority, autoAck: true);
                Assert.That(basicGetMsg, Is.Null);
            }
        }

        /// <summary>
        /// Publishings the message to fanout exchange publishes to all queues.
        /// </summary>
        [Test]
        public void Publishing_message_to_fanout_exchange_publishes_to_all_queues()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.RegisterFanoutExchange(ExchangeFanout);

                RegisterQueue(channel, QueueNames<HelloRabbit>.In, exchange: ExchangeFanout);
                RegisterQueue(channel, QueueNames<HelloRabbit>.Priority, exchange: ExchangeFanout);

                byte[] payload = new HelloRabbit { Name = "World!" }.ToJson().ToUtf8Bytes();
                var props = channel.CreateBasicProperties();
                props.Persistent = true;

                channel.BasicPublish(ExchangeFanout, QueueNames<HelloRabbit>.In, props, payload);

                var basicGetMsg = channel.BasicGet(QueueNames<HelloRabbit>.In, autoAck: true);
                Assert.That(basicGetMsg, Is.Not.Null);

                basicGetMsg = channel.BasicGet(QueueNames<HelloRabbit>.Priority, autoAck: true);
                Assert.That(basicGetMsg, Is.Not.Null);
            }
        }


        /// <summary>
        /// Doeses the publish to dead letter exchange.
        /// </summary>
        [Test]
        public void Does_publish_to_dead_letter_exchange()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.OpenChannel())
            {
                PublishHelloRabbit(channel);

                var basicGetMsg = channel.BasicGet(QueueNames<HelloRabbit>.In, autoAck: true);
                var dlqBasicMsg = channel.BasicGet(QueueNames<HelloRabbit>.Dlq, autoAck: true);
                Assert.That(basicGetMsg, Is.Not.Null);
                Assert.That(dlqBasicMsg, Is.Null);

                PublishHelloRabbit(channel);

                basicGetMsg = channel.BasicGet(QueueNames<HelloRabbit>.In, autoAck: false);
                Thread.Sleep(500);
                dlqBasicMsg = channel.BasicGet(QueueNames<HelloRabbit>.Dlq, autoAck: false);
                Assert.That(basicGetMsg, Is.Not.Null);
                Assert.That(dlqBasicMsg, Is.Null);

                channel.BasicNack(basicGetMsg.DeliveryTag, multiple: false, requeue: false);

                Thread.Sleep(500);
                dlqBasicMsg = channel.BasicGet(QueueNames<HelloRabbit>.Dlq, autoAck: true);
                Assert.That(dlqBasicMsg, Is.Not.Null);
            }
        }

        /// <summary>
        /// Determines whether this instance [can interrupt basic consumer in bgthread by closing channel].
        /// </summary>
        [Test]
        public void Can_interrupt_BasicConsumer_in_bgthread_by_closing_channel()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                string recvMsg = null;
                EndOfStreamException lastEx = null;

                var bgThread = new Thread(() =>
                {
                    try
                    {
                        //Url：https://stackoverflow.com/questions/71046474/unit-test-running-forever-for-call-back-method-in-c-sharp/71048247
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
                                catch (EndOfStreamException ex)
                                {
                                    // The consumer was cancelled, the model closed, or the
                                    // connection went away.
                                    Console.WriteLine($"EndOfStreamException in bgthread: {ex.Message}");
                                    lastEx = ex;
                                    Assert.That(lastEx, Is.Not.Null);
                                }
                                catch (Exception ex)
                                {
                                    Assert.Fail("Unexpected exception in bgthread: " + ex.Message);
                                }

                                finally
                                {
                                    barrier.Set(); // Signal Event fired.
                                }
                               
                            };
                            barrier.Wait(); // Wait for Event to fire.
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception in bgthread: {ex.GetType().Name}: {ex.Message}");
                    }
                })
                {
                    Name = "Closing Channel Test",
                    IsBackground = true,
                };
                bgThread.Start();

                PublishHelloRabbit(channel);
                Thread.Sleep(100);

                //closing either throws EndOfStreamException in bgthread
                channel.Close();
                //connection.Close();

                Thread.Sleep(2000);



                Console.WriteLine("EOF...");
            }
        }

        /// <summary>
        /// Determines whether this instance [can consume messages with basic consumer].
        /// </summary>
        [Test]
        public void Can_consume_messages_with_BasicConsumer()
        {
            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                OperationInterruptedException lastEx = null;

                channel.Close();

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        PublishHelloRabbit(channel);
                    }
                    catch (Exception ex)
                    {
                        lastEx = ex as OperationInterruptedException;
                        Console.WriteLine($"Caught {ex.GetType().Name}: {ex}");
                    }
                });

                Thread.Sleep(1000);

                Assert.That(lastEx, Is.Not.Null);

                Console.WriteLine("EOF...");
            }
        }

        /// <summary>
        /// Deletes all queues and exchanges.
        /// </summary>
        [Test]
        public void Delete_all_queues_and_exchanges()
        {
            var exchangeNames = new[] {
                Exchange,
                ExchangeDlq,
                ExchangeTopic,
                ExchangeFanout,
                QueueNames.Exchange,
                QueueNames.ExchangeDlq,
                QueueNames.ExchangeTopic,
            };

            using (IConnection connection = mqFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                exchangeNames.Each(x => channel.ExchangeDelete(x));

                channel.DeleteQueue<AlwaysThrows>();
                channel.DeleteQueue<Hello>();
                channel.DeleteQueue<HelloRabbit>();
                channel.DeleteQueue<HelloResponse>();
                channel.DeleteQueue<Incr>();
                channel.DeleteQueue<AnyTestMq>();
                channel.DeleteQueue<AnyTestMqResponse>();
                channel.DeleteQueue<PostTestMq>();
                channel.DeleteQueue<PostTestMqResponse>();
                channel.DeleteQueue<ValidateTestMq>();
                channel.DeleteQueue<ValidateTestMqResponse>();
                channel.DeleteQueue<ThrowGenericError>();
                channel.DeleteQueue<Reverse>();
                channel.DeleteQueue<Rot13>();
                channel.DeleteQueue<Wait>();
            }
        }

       
    }
    //Dummy messages to delete Queue's created else where.    
    /// <summary>
    /// 
    /// </summary>
    public class Incr
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AlwaysThrows { }
    /// <summary>
    /// 
    /// </summary>
    public class Hello { }
    /// <summary>
    /// 
    /// </summary>
    public class HelloResponse { }
    /// <summary>
    /// 
    /// </summary>
    public class Reverse { }
    /// <summary>
    /// 
    /// </summary>
    public class Rot13 { }
    /// <summary>
    /// 
    /// </summary>
    public class Wait { }


    /// <summary>
    /// 
    /// </summary>
    public class AnyTestMq
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnyTestMqAsync
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnyTestMqResponse
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        public int CorrelationId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PostTestMq
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }
    /// <summary>
    /// /
    /// </summary>
    public class PostTestMqResponse
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        public int CorrelationId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ValidateTestMq
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ValidateTestMqResponse
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        public int CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the response status.
        /// </summary>
        /// <value>
        /// The response status.
        /// </value>
       public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ThrowGenericError
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }

}
