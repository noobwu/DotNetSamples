//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

namespace NoobCore.Model
{
    /// <summary>
    /// Allow Exceptions to Customize ResponseStatus returned    
    /// </summary>
    public interface IResponseStatusConvertible
    {
        /// <summary>
        /// Converts to responsestatus.
        /// </summary>
        /// <returns></returns>
        ResponseStatus ToResponseStatus();
    }
}