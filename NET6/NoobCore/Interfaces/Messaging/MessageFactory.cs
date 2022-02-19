using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="body">The body.</param>
    /// <returns></returns>
    internal delegate IMessage MessageFactoryDelegate(object body);
    /// <summary>
    /// 
    /// </summary>
    public static class MessageFactory
    {
        /// <summary>
        /// The cache function
        /// </summary>
        static readonly Dictionary<Type, MessageFactoryDelegate> CacheFn
            = new Dictionary<Type, MessageFactoryDelegate>();
        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static IMessage Create(object response)
        {
            if (response is IMessage responseMessage)
            {
                return responseMessage;
            }

            if (response == null)
            {
                return null;
            }
            var type = response.GetType();

            MessageFactoryDelegate factoryFn;
            lock (CacheFn) CacheFn.TryGetValue(type, out factoryFn);

            if (factoryFn != null)
            {
                return factoryFn(response);
            }
            var genericMessageType = typeof(Message<>).MakeGenericType(type);
            var mi = genericMessageType.GetMethod("Create",
                BindingFlags.Public | BindingFlags.Static);
            factoryFn = (MessageFactoryDelegate)Delegate.CreateDelegate(
                typeof(MessageFactoryDelegate), mi);

            lock (CacheFn) CacheFn[type] = factoryFn;

            return factoryFn(response);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.Messaging.IMessage" />
    public class Message : IMessage
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public long Priority { get; set; }
        /// <summary>
        /// Gets or sets the retry attempts.
        /// </summary>
        /// <value>
        /// The retry attempts.
        /// </value>
        public int RetryAttempts { get; set; }
        /// <summary>
        /// Gets or sets the reply identifier.
        /// </summary>
        /// <value>
        /// The reply identifier.
        /// </value>
        public Guid? ReplyId { get; set; }
        /// <summary>
        /// Gets or sets the reply to.
        /// </summary>
        /// <value>
        /// The reply to.
        /// </value>
        public string ReplyTo { get; set; }
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public int Options { get; set; }
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public ResponseStatus Error { get; set; }
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>
        /// The meta.
        /// </value>
        public Dictionary<string, string> Meta { get; set; }
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public object Body { get; set; }
    }

    /// <summary>
    /// Basic implementation of IMessage[T]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Message<T>: Message, IMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message{T}"/> class.
        /// </summary>
        public Message()
        {
            this.Id = Guid.NewGuid();
            this.CreatedDate = DateTime.UtcNow;
            this.Options = (int)MessageOption.NotifyOneWay;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message{T}"/> class.
        /// </summary>
        /// <param name="body">The body.</param>
        public Message(T body): this()
        {
            Body = body;
        }

        /// <summary>
        /// Creates the specified o body.
        /// </summary>
        /// <param name="oBody">The o body.</param>
        /// <returns></returns>
        public static IMessage Create(object oBody)
        {
            return new Message<T>((T)oBody);
        }
        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <returns></returns>
        public T GetBody()
        {
            return (T)Body;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"CreatedDate={this.CreatedDate}, Id={this.Id:N}, Type={typeof(T).Name}, Retry={this.RetryAttempts}";
        }

    }
}
