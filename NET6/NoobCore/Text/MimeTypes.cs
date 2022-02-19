//Url：https://github.com/ServiceStack/ServiceStack.Text/blob/master/src/ServiceStack.Text/MimeTypes.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class MimeTypes
    {
        /// <summary>
        /// The json
        /// </summary>
        public const string Json = "application/json";

        /// <summary>
        /// Case-insensitive, trimmed compare of two content types from start to ';', i.e. without charset suffix 
        /// </summary>
        public static bool MatchesContentType(string contentType, string matchesContentType)
        {
            if (contentType == null || matchesContentType == null)
                return false;

            int start = -1, matchStart = -1, matchEnd = -1;

            for (var i = 0; i < contentType.Length; i++)
            {
                if (char.IsWhiteSpace(contentType[i]))
                    continue;
                start = i;
                break;
            }

            for (var i = 0; i < matchesContentType.Length; i++)
            {
                if (char.IsWhiteSpace(matchesContentType[i]))
                    continue;
                if (matchesContentType[i] == ';')
                    break;
                if (matchStart == -1)
                    matchStart = i;
                matchEnd = i;
            }

            return start != -1 && matchStart != -1 && matchEnd != -1
                   && string.Compare(contentType, start,
                       matchesContentType, matchStart, matchEnd - matchStart + 1,
                       StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
