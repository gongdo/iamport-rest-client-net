using System;

namespace Iamport.RestApi
{
    /// <summary>
    /// DateTime에 관한 확장 메서드를 제공하는 정적 클래스입니다.
    /// </summary>
    internal static class DateTimeExtensions
    {
        /// <summary>
        /// Unix timestamp를 DateTime으로 반환합니다.
        /// </summary>
        /// <param name="unixTime">Unix timestamp</param>
        /// <param name="includeMilliseconds">밀리초 단위인지 여부</param>
        /// <param name="kind">DateTime의 종류</param>
        /// <returns>변환된 DateTime</returns>
        public static DateTime FromUnixTime(this long unixTime, bool includeMilliseconds = true, DateTimeKind kind = DateTimeKind.Utc)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, kind);
            if (includeMilliseconds)
            {
                return epoch.AddMilliseconds(unixTime);
            }
            else
            {
                return epoch.AddSeconds(unixTime);
            }
        }

        /// <summary>
        /// DateTime을 Unix timestamp로 반환합니다.
        /// </summary>
        /// <param name="date">변환할 DateTime</param>
        /// <param name="includeMilliseconds">밀리초 단위인지 여부</param>
        /// <param name="kind">DateTime의 종류</param>
        /// <returns>변환된 Unix timestamp</returns>
        public static long ToUnixTime(this DateTime date, bool includeMilliseconds = true, DateTimeKind kind = DateTimeKind.Utc)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, kind);
            var diff = includeMilliseconds ? (date - epoch).TotalMilliseconds : (date - epoch).TotalSeconds;
#if DNXCORE50
            return (long)diff;
#else
            return (long)Math.Floor(diff);
#endif
        }
    }
}
