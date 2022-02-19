using System;
using NoobCore.Model;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.Model.IHasId&lt;System.Guid&gt;" />
    /// <seealso cref="NoobCore.IMeta" />
    public interface IMessage: IHasId<Guid>, IMeta
    {
        /// <summary>
        /// Gets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        DateTime CreatedDate { get; }
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        long Priority { get; set; }
        /// <summary>
        /// Gets or sets the retry attempts.
        /// </summary>
        /// <value>
        /// The retry attempts.
        /// </value>
        int RetryAttempts { get; set; }
        /// <summary>
        /// Gets or sets the reply identifier.
        /// </summary>
        /// <value>
        /// The reply identifier.
        /// </value>
        Guid? ReplyId { get; set; }
        /// <summary>
        /// Gets or sets the reply to.
        /// </summary>
        /// <value>
        /// The reply to.
        /// </value>
        string ReplyTo { get; set; }
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        int Options { get; set; }
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        ResponseStatus Error { get; set; }
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        string Tag { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        object Body { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="NoobCore.Model.IHasId&lt;System.Guid&gt;" />
    /// <seealso cref="NoobCore.IMeta" />
    public interface IMessage<T>: IMessage
    {
        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <returns></returns>
        T GetBody();
    }
}