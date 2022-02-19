//Url：https://github.com/ServiceStack/ServiceStack.Text/blob/master/src/ServiceStack.Text/DateTimeExtensions.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    /// <summary>
    /// A fast, standards-based, serialization-issue free DateTime serializer.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// The unix epoch
        /// </summary>
        public const long UnixEpoch = 621355968000000000L;
        /// <summary>
        /// The unix epoch date time UTC
        /// </summary>
        private static readonly DateTime UnixEpochDateTimeUtc = new DateTime(UnixEpoch, DateTimeKind.Utc);
        /// <summary>
        /// The unix epoch date time unspecified
        /// </summary>
        private static readonly DateTime UnixEpochDateTimeUnspecified = new DateTime(UnixEpoch, DateTimeKind.Unspecified);
        /// <summary>
        /// The minimum date time UTC
        /// </summary>
        private static readonly DateTime MinDateTimeUtc = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Froms the unix time.
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this int unixTime)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }
        /// <summary>
        /// Froms the unix time.
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this double unixTime)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }

        /// <summary>
        /// Froms the unix time.
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this long unixTime)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }

        /// <summary>
        /// Converts to unixtimemsalt.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long ToUnixTimeMsAlt(this DateTime dateTime)
        {
            return (dateTime.ToStableUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Converts to unixtimems.
        /// </summary>
        /// <param name="dateTimeOffset">The date time offset.</param>
        /// <returns></returns>
        public static long ToUnixTimeMs(this DateTimeOffset dateTimeOffset) =>
            (long)ToDateTimeSinceUnixEpoch(dateTimeOffset.UtcDateTime).TotalMilliseconds;

        /// <summary>
        /// Converts to unixtimems.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long ToUnixTimeMs(this DateTime dateTime)
        {
            var universal = ToDateTimeSinceUnixEpoch(dateTime);
            return (long)universal.TotalMilliseconds;
        }

        /// <summary>
        /// Converts to unixtime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime dateTime)
        {
            return (dateTime.ToDateTimeSinceUnixEpoch().Ticks) / TimeSpan.TicksPerSecond;
        }
        /// <summary>
        /// Gets the local time zone information.
        /// </summary>
        /// <returns></returns>
        public static TimeZoneInfo GetLocalTimeZoneInfo()
        {
            try
            {
                return TimeZoneInfo.Local;
            }
            catch (Exception)
            {
                return TimeZoneInfo.Utc; //Fallback for Mono on Windows.
            }
        }

        internal static TimeZoneInfo LocalTimeZone = GetLocalTimeZoneInfo();
        /// <summary>
        /// Converts to datetimesinceunixepoch.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        private static TimeSpan ToDateTimeSinceUnixEpoch(this DateTime dateTime)
        {
            var dtUtc = dateTime;
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dtUtc = dateTime.Kind == DateTimeKind.Unspecified && dateTime > DateTime.MinValue && dateTime < DateTime.MaxValue
                    ? DateTime.SpecifyKind(dateTime.Subtract(LocalTimeZone.GetUtcOffset(dateTime)), DateTimeKind.Utc)
                    : dateTime.ToStableUniversalTime();
            }

            var universal = dtUtc.Subtract(UnixEpochDateTimeUtc);
            return universal;
        }
        /// <summary>
        /// Converts to stableuniversaltime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static DateTime ToStableUniversalTime(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            if (dateTime == DateTime.MinValue)
                return MinDateTimeUtc;

            return dateTime.ToUniversalTime();
        }
        /// <summary>
        /// Converts to unixtimems.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns></returns>
        public static long ToUnixTimeMs(this long ticks)
        {
            return (ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }
        /// <summary>
        /// Converts to unixtimems.
        /// </summary>
        /// <param name="dateOnly">The date only.</param>
        /// <returns></returns>
        public static long ToUnixTimeMs(this DateOnly dateOnly) => dateOnly.ToDateTime(default, DateTimeKind.Utc).ToUnixTimeMs();
        /// <summary>
        /// Converts to unixtime.
        /// </summary>
        /// <param name="dateOnly">The date only.</param>
        /// <returns></returns>
        public static long ToUnixTime(this DateOnly dateOnly) => dateOnly.ToDateTime(default, DateTimeKind.Utc).ToUnixTime();
    }
}
