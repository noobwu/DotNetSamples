using System;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientFactory
    {
        /// <summary>
        /// Creates the specified endpoint URL.
        /// </summary>
        /// <param name="endpointUrl">The endpoint URL.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">could not find service client for " + endpointUrl</exception>
        public static IOneWayClient Create(string endpointUrl)
        {
            if (string.IsNullOrWhiteSpace(endpointUrl) || !endpointUrl.StartsWith("http"))
                return null;

            throw new NotImplementedException("could not find service client for " + endpointUrl);
        }
    }
}