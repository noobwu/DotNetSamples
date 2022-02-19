using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Messaging
{
    /// <summary>
    /// Util static generic class to create unique queue names for types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class QueueNames<T>
    {
        /// <summary>
        /// 
        /// </summary>
        static QueueNames()
        {
            Priority = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".priorityq");
            In = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".inq");
            Out = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".outq");
            Dlq = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".dlq");
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Priority { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static string In { get; private set; }
        /// <summary>
        /// 
        /// </summary>

        public static string Out { get; private set; }
        /// <summary>
        /// 
        /// </summary>

        public static string Dlq { get; private set; }
        /// <summary>
        /// 
        /// </summary>

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
        /// 
        /// </summary>
        public static string Exchange = "mx.noobcore";
        /// <summary>
        /// 
        /// </summary>
        public static string ExchangeDlq = "mx.noobcore.dlq";
        /// <summary>
        /// 
        /// </summary>
        public static string ExchangeTopic = "mx.noobcore.topic";

        /// <summary>
        /// 
        /// </summary>
        public static string MqPrefix = "mq:";
        /// <summary>
        /// 
        /// </summary>
        public static string QueuePrefix = "";
        /// <summary>
        /// 
        /// </summary>
        public static string TempMqPrefix = MqPrefix + "tmp:";
        /// <summary>
        /// 
        /// </summary>
        public static string TopicIn = MqPrefix + "topic:in";
        /// <summary>
        /// 
        /// </summary>
        public static string TopicOut = MqPrefix + "topic:out";

        /// <summary>
        /// 
        /// </summary>
        public static Func<string, string, string> ResolveQueueNameFn = ResolveQueueName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="queueSuffix"></param>
        /// <returns></returns>
        public static string ResolveQueueName(string typeName, string queueSuffix)
        {
            return QueuePrefix + MqPrefix + typeName + queueSuffix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static bool IsTempQueue(string queueName)
        {
            return queueName != null
                && queueName.StartsWith(TempMqPrefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        public static void SetQueuePrefix(string prefix)
        {
            TopicIn = prefix + MqPrefix + "topic:in";
            TopicOut = prefix + MqPrefix + "topic:out";
            QueuePrefix = prefix;
            TempMqPrefix = prefix + MqPrefix + "tmp:";
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly Type messageType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
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
