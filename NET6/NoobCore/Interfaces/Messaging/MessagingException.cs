using System;
using NoobCore.Model;

namespace NoobCore.Messaging
{
    public class MessagingException : Exception, IHasResponseStatus, IResponseStatusConvertible
    {
        public MessagingException() {}

        public MessagingException(string message) : base(message) {}

        public MessagingException(string message, Exception innerException) : base(message, innerException) {}

        public MessagingException(ResponseStatus responseStatus, Exception innerException = null)
            : base(responseStatus.Message ?? responseStatus.ErrorCode, innerException)
        {
            ResponseStatus = responseStatus;
        }

        public MessagingException(ResponseStatus responseStatus, object responseDto, Exception innerException = null)
            : this(responseStatus, innerException)
        {
            ResponseDto = responseDto;
        }
        /// <summary>
        /// Gets or sets the response dto.
        /// </summary>
        /// <value>
        /// The response dto.
        /// </value>
        public object ResponseDto { get; set; }

        /// <summary>
        /// Gets or sets the response status.
        /// </summary>
        /// <value>
        /// The response status.
        /// </value>
        public ResponseStatus ResponseStatus { get; set; }

        /// <summary>
        /// Converts to responsestatus.
        /// </summary>
        /// <returns></returns>
        public ResponseStatus ToResponseStatus()
        {
            return ResponseStatus;
        }
    }
}