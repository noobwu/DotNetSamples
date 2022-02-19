using NoobCore.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Converts to messagefncache.
        /// </summary>
        private static Dictionary<Type, ToMessageDelegate> ToMessageFnCache = new Dictionary<Type, ToMessageDelegate>();

        /// <summary>
        /// Gets to message function.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal static ToMessageDelegate GetToMessageFn(Type type)
        {
            ToMessageFnCache.TryGetValue(type, out var toMessageFn);

            if (toMessageFn != null) return toMessageFn;

            var genericType = typeof(MessageExtensions<>).MakeGenericType(type);
            var mi = genericType.GetStaticMethod("ConvertToMessage");
            toMessageFn = (ToMessageDelegate)mi.MakeDelegate(typeof(ToMessageDelegate));

            Dictionary<Type, ToMessageDelegate> snapshot, newCache;
            do
            {
                snapshot = ToMessageFnCache;
                newCache = new Dictionary<Type, ToMessageDelegate>(ToMessageFnCache)
                {
                    [type] = toMessageFn
                };

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ToMessageFnCache, newCache, snapshot), snapshot));

            return toMessageFn;
        }

        /// <summary>
        /// Converts to message.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="ofType">Type of the of.</param>
        /// <returns></returns>
        public static IMessage ToMessage(this byte[] bytes, Type ofType)
        {
            if (bytes == null)
                return null;

            var msgFn = GetToMessageFn(ofType);
            var msg = msgFn(bytes);
            return msg;
        }
        /// <summary>
        /// Converts to message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static Message<T> ToMessage<T>(this byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            var messageText = ToString(bytes);
            
            if (string.IsNullOrWhiteSpace(messageText)) {
                return null;
            }
            return JsonSerializer.Deserialize<Message<T>>(messageText);
        }

        /// <summary>
        /// Converts to bytes.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static byte[] ToBytes(this IMessage message)
        {
            var serializedMessage = JsonSerializer.Serialize((object)message);
            return System.Text.Encoding.UTF8.GetBytes(serializedMessage);
        }

        public static byte[] ToBytes<T>(this IMessage<T> message)
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            return System.Text.Encoding.UTF8.GetBytes(serializedMessage);
        }
        /// <summary>
        /// Converts to inqueuename.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string ToInQueueName(this IMessage message)
        {
            var queueName = message.Priority > 0
                ? new QueueNames(message.Body.GetType()).Priority
                : new QueueNames(message.Body.GetType()).In;

            return queueName;
        }

        /// <summary>
        /// Converts to dlqqueuename.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string ToDlqQueueName(this IMessage message)
        {
            return new QueueNames(message.Body.GetType()).Dlq;
        }

        /// <summary>
        /// Converts to inqueuename.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string ToInQueueName<T>(this IMessage<T> message)
        {
            return message.Priority > 0
                ? QueueNames<T>.Priority
                : QueueNames<T>.In;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="param">The parameter.</param>
    /// <returns></returns>
    internal delegate IMessage ToMessageDelegate(object param);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class MessageExtensions<T>
    {
        public static IMessage ConvertToMessage(object oBytes)
        {
            var bytes = (byte[])oBytes;
            return bytes.ToMessage<T>();
        }
    }
}
