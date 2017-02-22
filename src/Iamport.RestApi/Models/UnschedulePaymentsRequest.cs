using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트의 subscribe-api에 schedule된 결제를 취소할 때 입력할 정보
    /// </summary>
    /// <seealso href="http://api.iamport.kr/#!/subscribe"/>
    public class UnschedulePaymentsRequest
    {
        /// <summary>
        /// (required)고객 고유번호(빌링키에 대응하는 고객 식별 문자열)
        /// </summary>
        [JsonProperty("customer_uid")]
        [Required]
        [MaxLength(80)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 스케줄로 등록한 거래 고유 ID의 목록.
        /// (누락되면 customer_uid에 대한 결제예약정보 일괄취소)
        /// </summary>
        [JsonProperty("merchant_uid")]
        public string[] TransactionIds { get; set; }
    }
}
