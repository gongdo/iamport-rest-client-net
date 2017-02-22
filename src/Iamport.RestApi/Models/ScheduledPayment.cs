using Iamport.RestApi.JsonConverters;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트의 subscribe-api에서 등록된 schedule 결제 정보
    /// </summary>
    /// <seealso href="http://api.iamport.kr/#!/subscribe"/>
    public class ScheduledPayment
    {
        /// <summary>
        /// 고객 고유번호(빌링키에 대응하는 고객 식별 문자열)
        /// </summary>
        [JsonProperty("customer_uid")]
        [MaxLength(80)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 이 결제를 거래할 때 사용할 고유 ID(OrderId 등)
        /// </summary>
        [JsonProperty("merchant_uid")]
        [MaxLength(80)]
        public string TransactionId { get; set; }
        /// <summary>
        /// 결제 총액
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        
        /// <summary>
        /// 결제요청 예약시각 UNIX timestamp(UTC)
        /// </summary>
        [JsonProperty("schedule_at")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime ScheduleAt { get; set; }
        /// <summary>
        /// 주문이름
        /// </summary>
        [JsonProperty("name")]
        [MaxLength(80)]
        public string Title { get; set; }
        /// <summary>
        /// (optional)주문자 이름
        /// </summary>
        [JsonProperty("buyer_name")]
        public string BuyerName { get; set; }
        /// <summary>
        /// (optional)주문자 전화번호
        /// </summary>
        [JsonProperty("buyer_tel")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// (optional)주문자 이메일
        /// </summary>
        [JsonProperty("buyer_email")]
        public string Email { get; set; }
        /// <summary>
        /// (optional)주문자 주소
        /// </summary>
        [JsonProperty("buyer_addr")]
        public string Address { get; set; }
        /// <summary>
        /// (optional)주문자 우편번호
        /// </summary>
        [JsonProperty("buyer_postcode")]
        public string PostCode { get; set; }
    }
}
