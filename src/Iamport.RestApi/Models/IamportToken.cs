using Iamport.RestApi.JsonConverters;
using Newtonsoft.Json;
using System;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트 보안 토큰 정보를 정의하는 클래스입니다.
    /// </summary>
    public class IamportToken
    {
        /// <summary>
        /// API 액세스 토큰
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        /// <summary>
        /// 토큰 발급일시(UTC)
        /// </summary>
        [JsonProperty("now")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime IssuedAt { get; set; }
        /// <summary>
        /// 토큰 만료일시(UTC)
        /// </summary>
        [JsonProperty("expired_at")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime ExpiredAt { get; set; }
    }
}
