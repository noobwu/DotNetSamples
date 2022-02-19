using NoobCore.Model;
using NoobCore.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    public static class DtoUtils
    {
        /// <summary>
        /// Naming convention for the ResponseStatus property name on the response DTO
        /// </summary>
        public const string ResponseStatusPropertyName = "ResponseStatus";
        /// <summary>
        /// Creates the response status.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="request">The request.</param>
        /// <param name="debugMode">if set to <c>true</c> [debug mode].</param>
        /// <returns></returns>
        public static ResponseStatus CreateResponseStatus(Exception ex, object request = null, bool debugMode = false)
        {
            var e = ex.UnwrapIfSingleException();

            var responseStatus = (e is IResponseStatusConvertible customStatus
                ? customStatus.ToResponseStatus()
                : null) ?? ResponseStatusUtils.CreateResponseStatus(e.GetType().Name, e.Message);

            if (responseStatus == null)
                return null;

            if (debugMode)
            {
                // View stack trace in tests and on the client
                var sb = StringBuilderCache.Allocate();

                if (request != null)
                {
                    try
                    {
                        var str = $"[{request.GetType().GetOperationName()}: {DateTime.UtcNow}]:\n[REQUEST: {TypeSerializer.SerializeToString(request)}]";
                        sb.AppendLine(str);
                    }
                    catch (Exception requestEx)
                    {
                        sb.AppendLine($"[{request.GetType().GetOperationName()}: {DateTime.UtcNow}]:\n[REQUEST: {requestEx.Message}]");
                    }
                }

                sb.AppendLine(e.ToString());

                var innerMessages = new List<string>();
                var innerEx = e.InnerException;
                while (innerEx != null)
                {
                    sb.AppendLine("");
                    sb.AppendLine(innerEx.ToString());
                    innerMessages.Add(innerEx.Message);
                    innerEx = innerEx.InnerException;
                }

                responseStatus.StackTrace = StringBuilderCache.ReturnAndFree(sb);
                if (innerMessages.Count > 0)
                {
                    responseStatus.Meta ??= new Dictionary<string, string>();
                    responseStatus.Meta["InnerMessages"] = innerMessages.Join("\n");
                }
            }

            return responseStatus;
        }

        /// <summary>
        /// Converts to responsestatus.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="requestDto">The request dto.</param>
        /// <returns></returns>
        public static ResponseStatus ToResponseStatus(this Exception exception, object requestDto = null)
        {
            return CreateResponseStatus(exception, requestDto);
        }
    }
}
