using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    public static class ContentFormat
    {
        /// <summary>
        /// The UTF8 suffix
        /// </summary>
        public const string Utf8Suffix = "; charset=utf-8";
        /// <summary>
        /// Matcheses the type of the content.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="matchesContentType">Type of the matches content.</param>
        /// <returns></returns>
        public static bool MatchesContentType(this string contentType, string matchesContentType) =>
         MimeTypes.MatchesContentType(contentType, matchesContentType);
    }
}
