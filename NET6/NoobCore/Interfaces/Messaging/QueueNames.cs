using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class QueueNames<T>
    {
        /// <summary>
        /// Initializes the <see cref="QueueNames{T}"/> class.
        /// </summary>
        static QueueNames()
        {
            Priority = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".priorityq");
            In = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".inq");
            Out = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".outq");
            Dlq = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".dlq");
        }

        /// <summary>
        /// 优先较高的队列名称
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public static string Priority { get; private set; }

        /// <summary>
        /// 点对点队列模式队列名称
        /// </summary>
        /// <value>
        /// The in.
        /// </value>
        public static string In { get; private set; }

        /// <summary>
        /// 主题订阅模式队列名称
        /// </summary>
        /// <value>
        /// The out.
        /// </value>
        public static string Out { get; private set; }

        /// <summary>
        /// Gets the DLQ.
        /// </summary>
        /// <value>
        /// The DLQ.
        /// </value>
        public static string Dlq { get; private set; }

        /// <summary>
        /// Gets all queue names.
        /// </summary>
        /// <value>
        /// All queue names.
        /// </value>
        public static string[] AllQueueNames
        {
            get
            {
                return new[] {
                    In,
                    Priority,
                    Out,
                    Dlq,
                };
            }
        }
    }

    /// <summary>
    /// Util class to create unique queue names for runtime types
    /// </summary>
    public class QueueNames
    {
        /// <summary>
        /// The exchange
        /// </summary>
        public static string Exchange = "mx.noobcore";
        /// <summary>
        /// The exchange DLQ
        /// </summary>
        public static string ExchangeDlq = "mx.noobcore.dlq";
        /// <summary>
        /// The exchange topic
        /// </summary>
        public static string ExchangeTopic = "mx.noobcore.topic";

        /// <summary>
        /// The mq prefix
        /// </summary>
        public static string MqPrefix = "mq:";
        /// <summary>
        /// The queue prefix
        /// </summary>
        public static string QueuePrefix = "";
        /// <summary>
        /// The temporary mq prefix
        /// </summary>
        public static string TempMqPrefix = MqPrefix + "tmp:";
        /// <summary>
        /// The topic in
        /// </summary>
        public static string TopicIn = MqPrefix + "topic:in";
        /// <summary>
        /// The topic out
        /// </summary>
        public static string TopicOut = MqPrefix + "topic:out";

        /// <summary>
        /// The resolve queue name function
        /// </summary>
        public static Func<string, string, string> ResolveQueueNameFn = ResolveQueueName;

        /// <summary>
        /// Resolves the name of the queue.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="queueSuffix">The queue suffix.</param>
        /// <returns></returns>
        public static string ResolveQueueName(string typeName, string queueSuffix)
        {
            return QueuePrefix + MqPrefix + typeName + queueSuffix;
        }

        /// <summary>
        /// Determines whether [is temporary queue] [the specified queue name].
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>
        ///   <c>true</c> if [is temporary queue] [the specified queue name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTempQueue(string queueName)
        {
            return queueName != null
                && queueName.StartsWith(TempMqPrefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Sets the queue prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public static void SetQueuePrefix(string prefix)
        {
            TopicIn = prefix + MqPrefix + "topic:in";
            TopicOut = prefix + MqPrefix + "topic:out";
            QueuePrefix = prefix;
            TempMqPrefix = prefix + MqPrefix + "tmp:";
        }

        /// <summary>
        /// The message type
        /// </summary>
        private readonly Type messageType;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueNames"/> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        public QueueNames(Type messageType)
        {
            this.messageType = messageType;
        }

        /// <summary>
        /// 优先级队列名称
        /// </summary>
        public string Priority => ResolveQueueNameFn(messageType.Name, ".priorityq");

        /// <summary>
        /// 普通队列名称
        /// </summary>
        public string In => ResolveQueueNameFn(messageType.Name, ".inq");

        /// <summary>
        /// 订阅发布模式队列名称
        /// </summary>
        public string Out => ResolveQueueNameFn(messageType.Name, ".outq");

        /// <summary>
        /// 死信队列名称
        /// </summary>
        public string Dlq => ResolveQueueNameFn(messageType.Name, ".dlq");
                
        /// <summary>
        /// 临时队列名称
        /// </summary>
        /// <returns></returns>
        public static string GetTempQueueName()
        {
            return TempMqPrefix + Guid.NewGuid().ToString("n");
        }
    }
}
