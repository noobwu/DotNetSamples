// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt


using System.Collections.Generic;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeta
    {
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>
        /// The meta.
        /// </value>
        Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IHasSessionId
    {
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        string SessionId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IHasBearerToken
    {
        string BearerToken { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IHasRefreshToken
    {
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        string RefreshToken { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IHasVersion
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        int Version { get; set; }
    }
}