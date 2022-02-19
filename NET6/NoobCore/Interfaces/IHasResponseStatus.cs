//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

namespace NoobCore
{
    /// <summary>
    /// Contract indication that the Response DTO has a ResponseStatus
    /// </summary>
    public interface IHasResponseStatus
    {
        /// <summary>
        /// Gets or sets the response status.
        /// </summary>
        /// <value>
        /// The response status.
        /// </value>
        ResponseStatus ResponseStatus { get; set; }
    }
}