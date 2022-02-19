using System.Collections.Generic;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOneWayClient
    {
        /// <summary>
        /// Sends the one way.
        /// </summary>
        /// <param name="requestDto">The request dto.</param>
        void SendOneWay(object requestDto);

        /// <summary>
        /// Sends the one way.
        /// </summary>
        /// <param name="relativeOrAbsoluteUri">The relative or absolute URI.</param>
        /// <param name="requestDto">The request dto.</param>
        void SendOneWay(string relativeOrAbsoluteUri, object requestDto);

        /// <summary>
        /// Sends all one way.
        /// </summary>
        /// <param name="requests">The requests.</param>
        void SendAllOneWay(IEnumerable<object> requests);
    }
}