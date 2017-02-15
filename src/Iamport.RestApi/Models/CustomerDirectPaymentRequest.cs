using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트의 subscribe-api에 again 결제를 요청할 때 입력할 정보
    /// </summary>
    /// <seealso href="http://api.iamport.kr/#!/subscribe"/>
    public class CustomerDirectPaymentRequest
    {
        /// <summary>
        /// (required)고객 고유번호(빌링키에 대응하는 고객 식별 문자열)
        /// </summary>
        [JsonProperty("customer_uid ")]
        [Required]
        [MaxLength(80)]
        public string CustomerId { get; set; }
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
        /// <summary>
        /// 결제금액 중 부가세 금액(파라메터가 누락되면 10%로 자동 계산됨)
        /// </summary>
        [JsonProperty("vat")]
        public decimal Vat { get; set; }
        /// <summary>
        /// (optional)카드할부개월수. 2 이상의 integer 할부개월수 적용(결제금액 50,000원 이상 한정)
        /// </summary>
        [JsonProperty("card_quota")]
        public string InstallmentMonths { get; set; }
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
