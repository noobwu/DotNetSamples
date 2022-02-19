//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NoobCore
{
    /// <summary>
    /// Error information pertaining to a particular named field.
    /// Used for returning multiple field validation errors.s
    /// </summary>
    [DataContract]
    public class ResponseError : IMeta
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [DataMember(Order = 1)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        [DataMember(Order = 2)]
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember(Order = 3)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>
        /// The meta.
        /// </value>
        [DataMember(Order = 4)]
        public Dictionary<string, string> Meta { get; set; }
    }
}
