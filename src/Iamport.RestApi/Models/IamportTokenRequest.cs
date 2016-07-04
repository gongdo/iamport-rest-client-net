using Newtonsoft.Json;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트 토큰 요청 정보를 정의하는 클래스입니다.
    /// </summary>
    public class IamportTokenRequest
    {
        /// <summary>
        /// API 키
        /// </summary>
        [JsonProperty("imp_key")]
        public string ApiKey { get; set; }
        /// <summary>
        /// API 비밀번호
        /// </summary>
        [JsonProperty("imp_secret")]
        public string ApiSecret { get; set; }
    }
}
