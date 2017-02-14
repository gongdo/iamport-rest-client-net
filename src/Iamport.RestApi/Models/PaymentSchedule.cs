using Iamport.RestApi.JsonConverters;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트의 subscribe-api에서 사용하는 schedule 입력 정보
    /// </summary>
    /// <seealso href="http://api.iamport.kr/#!/subscribe"/>
    public class PaymentSchedule
    {
        /// <summary>
        /// 이 결제를 거래할 때 사용할 고유 ID(OrderId 등)
        /// </summary>
        [JsonProperty("merchant_uid")]
        [Required]
        [MaxLength(80)]
        public string TransactionId { get; set; }
        /// <summary>
        /// 결제 총액
        /// </summary>
        [JsonProperty("amount")]
        [Required]
        [Range(1000, 10000000)]
        public decimal Amount { get; set; }
        
        // - 스케줄에 부가세 항목이 빠져 있음.
        // - iamport의 실수라고 생각되지만 일단 사용하지 않음.
        ///// <summary>
        ///// 결제금액 중 부가세 금액(파라메터가 누락되면 10%로 자동 계산됨)
        ///// </summary>
        //[JsonProperty("vat")]
        //public decimal Vat { get; set; }

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
