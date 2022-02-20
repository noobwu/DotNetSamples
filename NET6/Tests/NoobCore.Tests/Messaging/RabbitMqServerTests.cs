using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoobCore.Logging;
using NoobCore.Messaging;
using NoobCore.RabbitMq;
using NUnit.Framework;
using RabbitMQ.Client;
using static NoobCore.Tests.Messaging.RabbitMqTests;

namespace NoobCore.Tests.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class Reverse
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Rot13
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AlwaysThrows
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }

    [TestFixture, Category("Integration")]
    public class RabbitMqServerTests
    {
        /// <summary>
        /// The connection string
        /// </summary>
        static readonly string ConnectionString = Config.RabbitMQConnString;

        /// <summary>
        /// Tests the fixture set up.
        /// </summary>
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            LogManager.LogFactory = new ConsoleLogFactory();
        }

        /// <summary>
        /// Creates the mq server.
        /// </summary>
        /// <param name="noOfRetries">The no of retries.</param>
        /// <returns></returns>
        internal static RabbitMqServer CreateMqServer(int noOfRetries = 2)
        {
            var mqServer = new RabbitMqServer(ConnectionString);
            using var conn = mqServer.ConnectionFactory.CreateConnection();
            using var channel = conn.CreateModel();
            channel.PurgeQueue<Reverse>();
            channel.PurgeQueue<Rot13>();
            channel.PurgeQueue<Incr>();
            channel.PurgeQueue<Wait>();
            channel.PurgeQueue<Hello>();
            channel.PurgeQueue<HelloResponse>();
            channel.PurgeQueue<HelloNull>();
            return mqServer;
        }

        /// <summary>
        /// Publishes the 4 messages.
        /// </summary>
        /// <param name="mqClient">The mq client.</param>
        internal static void Publish_4_messages(IMessageQueueClient mqClient)
        {
            mqClient.Publish(new Reverse { Value = "Hello" });
            mqClient.Publish(new Reverse { Value = "World" });
            mqClient.Publish(new Reverse { Value = "ServiceStack" });
            mqClient.Publish(new Reverse { Value = "Redis" });
        }


        /// <summary>
        /// Publishes the 4 rot13 messages.
        /// </summary>
        /// <param name="mqClient">The mq client.</param>
        private static void Publish_4_Rot13_messages(IMessageQueueClient mqClient)
        {
            mqClient.Publish(new Rot13 { Value = "Hello" });
            mqClient.Publish(new Rot13 { Value = "World" });
            mqClient.Publish(new Rot13 { Value = "ServiceStack" });
            mqClient.Publish(new Rot13 { Value = "Redis" });
        }

        /// <summary>
        /// Utilses the publish reverse messages.
        /// </summary>
        [Test]
        public void Utils_publish_Reverse_messages()
        {
            using (var mqHost = new RabbitMqServer(ConnectionString))
            using (var mqClient = mqHost.CreateMessageQueueClient())
            {
                Publish_4_messages(mqClient);
            }
        }


        /// <summary>
        /// Utilses the publish rot13 messages.
        /// </summary>
        [Test]
        public void Utils_publish_Rot13_messages()
        {
            using (var mqHost = new RabbitMqServer(ConnectionString))
            using (var mqClient = mqHost.CreateMessageQueueClient())
            {
                Publish_4_Rot13_messages(mqClient);
            }
        }

        /// <summary>
        /// Called when [allows 1 bg thread to run at a time].
        /// </summary>
        [Test]
        public void Only_allows_1_BgThread_to_run_at_a_time()
        {
            using (var mqHost = CreateMqServer())
            {
                mqHost.RegisterHandler<Reverse>(x => x.GetBody().Value.Reverse());
                mqHost.RegisterHandler<Rot13>(x => x.GetBody().Value.ToRot13());

                5.Times(x => ThreadPool.QueueUserWorkItem(y => mqHost.Start()));
                ExecUtils.RetryOnException(() =>
                {
                    Thread.Sleep(100);
                    Assert.That(mqHost.GetStatus(), Is.EqualTo("Started"));
                    Assert.That(mqHost.BgThreadCount, Is.EqualTo(1));
                    Thread.Sleep(100);
                }, TimeSpan.FromSeconds(5));

                10.Times(x => ThreadPool.QueueUserWorkItem(y => mqHost.Stop()));
                ExecUtils.RetryOnException(() =>
                {
                    Thread.Sleep(100);
                    Assert.That(mqHost.GetStatus(), Is.EqualTo("Stopped"));
                    Thread.Sleep(100);
                }, TimeSpan.FromSeconds(5));

                ThreadPool.QueueUserWorkItem(y => mqHost.Start());
                ExecUtils.RetryOnException(() =>
                {
                    Thread.Sleep(100);
                    Assert.That(mqHost.GetStatus(), Is.EqualTo("Started"));
                    Assert.That(mqHost.BgThreadCount, Is.EqualTo(2));
                    Thread.Sleep(100);
                }, TimeSpan.FromSeconds(5));

                //Debug.WriteLine(mqHost.GetStats());
            }
        }


        /// <summary>
        /// Cannots the start a disposed mq host.
        /// </summary>
        [Test]
        public void Cannot_Start_a_Disposed_MqHost()
        {
            var mqHost = CreateMqServer();

            mqHost.RegisterHandler<Reverse>(x => x.GetBody().Value.Reverse());
            mqHost.Dispose();

            try
            {
                mqHost.Start();
                Assert.Fail("Should throw ObjectDisposedException");
            }
            catch (ObjectDisposedException) { }
        }
        /// <summary>
        /// Cannots the stop a disposed mq host.
        /// </summary>
        [Test]
        public void Cannot_Stop_a_Disposed_MqHost()
        {
            var mqHost = CreateMqServer();

            mqHost.RegisterHandler<Reverse>(x => x.GetBody().Value.Reverse());
            mqHost.Start();
            Thread.Sleep(100);

            mqHost.Dispose();

            try
            {
                mqHost.Stop();
                Assert.Fail("Should throw ObjectDisposedException");
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// Determines whether this instance [can receive and process same reply responses].
        /// </summary>
        [Test]
        public void Can_receive_and_process_same_reply_responses()
        {
            var called = 0;
            using (var mqHost = CreateMqServer())
            {

                using (var conn = mqHost.ConnectionFactory.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    channel.PurgeQueue<Incr>();
                }

                mqHost.RegisterHandler<Incr>(m =>
                {
                    Debug.WriteLine("In Incr #" + m.GetBody().Value);
                    Interlocked.Increment(ref called);
                    return m.GetBody().Value > 0 ? new Incr { Value = m.GetBody().Value - 1 } : null;
                });

                mqHost.Start();

                var incr = new Incr { Value = 5 };
                using (var mqClient = mqHost.CreateMessageQueueClient())
                {
                    mqClient.Publish(incr);
                }

                ExecUtils.RetryOnException(() =>
                {
                    Thread.Sleep(300);
                    Assert.That(called, Is.EqualTo(1 + incr.Value));
                }, TimeSpan.FromSeconds(5));
            }
        }

        /// <summary>
        /// Determines whether this instance [can receive and process standard request reply combo].
        /// </summary>
        [Test]
        public void Can_receive_and_process_standard_request_reply_combo()
        {
            using (var mqHost = CreateMqServer())
            {
                using (var conn = mqHost.ConnectionFactory.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    channel.PurgeQueue<Hello>();
                    channel.PurgeQueue<HelloResponse>();
                }

                string messageReceived = null;

                mqHost.RegisterHandler<Hello>(m =>
                    new HelloResponse { Result = "Hello, " + m.GetBody().Name });

                mqHost.RegisterHandler<HelloResponse>(m =>
                {
                    messageReceived = m.GetBody().Result; return null;
                });

                mqHost.Start();

                using (var mqClient = mqHost.CreateMessageQueueClient())
                {
                    var dto = new Hello { Name = "ServiceStack" };
                    mqClient.Publish(dto);

                    ExecUtils.RetryOnException(() =>
                    {
                        Thread.Sleep(300);
                        Assert.That(messageReceived, Is.EqualTo("Hello, ServiceStack"));
                    }, TimeSpan.FromSeconds(5));
                }
            }
        }

        /// <summary>
        /// Determines whether this instance [can handle requests concurrently in 4 threads].
        /// </summary>
        [Test]
        public void Can_handle_requests_concurrently_in_4_threads()
        {
            RunHandlerOnMultipleThreads(noOfThreads: 4, msgs: 10);
        }
        /// <summary>
        /// Runs the handler on multiple threads.
        /// </summary>
        /// <param name="noOfThreads">The no of threads.</param>
        /// <param name="msgs">The MSGS.</param>
        private static void RunHandlerOnMultipleThreads(int noOfThreads, int msgs)
        {
            using var mqHost = CreateMqServer();
            var timesCalled = 0;

            mqHost.RegisterHandler<Wait>(m => {
                Interlocked.Increment(ref timesCalled);
                Thread.Sleep(m.GetBody().ForMs);
                return null;
            }, noOfThreads);

            mqHost.Start();

            using var mqClient = mqHost.CreateMessageQueueClient();
            var dto = new Wait { ForMs = 100 };
            msgs.Times(i => mqClient.Publish(dto));

            ExecUtils.RetryOnException(() =>
            {
                Thread.Sleep(300);
                Assert.That(timesCalled, Is.EqualTo(msgs));
            }, TimeSpan.FromSeconds(5));
        }
        /// <summary>
        /// 
        /// </summary>
        public class Wait
        {
            /// <summary>
            /// Gets or sets for ms.
            /// </summary>
            /// <value>
            /// For ms.
            /// </value>
            public int ForMs { get; set; }
        }

        /// <summary>
        /// Determines whether this instance [can publish and receive messages with message factory].
        /// </summary>
        [Test]
        public void Can_publish_and_receive_messages_with_MessageFactory()
        {
            using (var mqFactory = new RabbitMqMessageFactory(Config.RabbitMQConnString))
            using (var mqClient = mqFactory.CreateMessageQueueClient())
            {
                mqClient.Publish(new Hello { Name = "Foo" });
                var msg = mqClient.Get<Hello>(QueueNames<Hello>.In);

                Assert.That(msg.GetBody().Name, Is.EqualTo("Foo"));
            }
        }

        /// <summary>
        /// Determines whether this instance [can filter published and received messages].
        /// </summary>
        [Test]
        public void Can_filter_published_and_received_messages()
        {
            string receivedMsgApp = null;
            string receivedMsgType = null;
            IBasicProperties props = null;
            BasicGetResult basicMsgResult = null;

            using (var mqServer = CreateMqServer())
            {
                mqServer.PublishMessageFilter = (queueName, properties, msg) =>
                {
                    properties.AppId = $"app:{queueName}";
                };
                mqServer.GetMessageFilter = (queueName, basicMsg) =>
                {
                    basicMsgResult = basicMsg;
                    props = basicMsg.BasicProperties;
                    receivedMsgType = props.Type; //automatically added by RabbitMqProducer
                    receivedMsgApp = props.AppId;
                };

                mqServer.RegisterHandler<Hello>(m => {
                    return new HelloResponse { Result = $"Hello, {m.GetBody().Name}!" };
                });

                mqServer.Start();

                using (var mqClient = mqServer.CreateMessageQueueClient())
                {
                    mqClient.Publish(new Hello { Name = "Bugs Bunny" });
                }

                Thread.Sleep(100);

                Assert.That(receivedMsgApp, Is.EqualTo($"app:{QueueNames<Hello>.In}"));
                Assert.That(receivedMsgType, Is.EqualTo(typeof(Hello).Name));

                var queueName = QueueNames<HelloResponse>.In;
                if (props != null)
                {

                    Assert.That(props.Type, Is.EqualTo(typeof(HelloResponse).Name));
                    Assert.That(props.AppId, Is.EqualTo($"app:{queueName}"));
                }
                if (basicMsgResult != null)
                {
                    var msg = basicMsgResult.ToMessage<HelloResponse>();
                    Assert.That(msg.GetBody().Result, Is.EqualTo("Hello, Bugs Bunny!"));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="NoobCore.IReturn&lt;NoobCore.Tests.Messaging.RabbitMqServerTests.HelloResponse&gt;" />
        public class Hello : IReturn<HelloResponse>
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
        /// <seealso cref="NoobCore.IReturn&lt;NoobCore.Tests.Messaging.RabbitMqServerTests.HelloResponse&gt;" />
        public class HelloNull : IReturn<HelloResponse>
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
        public class HelloResponse
        {
            /// <summary>
            /// Gets or sets the result.
            /// </summary>
            /// <value>
            /// The result.
            /// </value>
            public string Result { get; set; }
        }
    }
}
