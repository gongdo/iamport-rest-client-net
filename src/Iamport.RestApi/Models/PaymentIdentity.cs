using Newtonsoft.Json;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 거래 고유 정보를 정의하는 클래스입니다.
    /// 이 정보는 return URL의 querystring 필드를 나타냅니다.
    /// </summary>
    public class PaymentIdentity
    {
        /// <summary>
        /// 아임포트의 거래 고유 번호
        /// </summary>
        [JsonProperty("imp_uid")]
        public string IamportId { get; set; }
        /// <summary>
        /// 각 상점에서 다루는 거래 고유 번호
        /// </summary>
        [JsonProperty("merchant_uid")]
        public string TransactionId { get; set; }
    }
}
