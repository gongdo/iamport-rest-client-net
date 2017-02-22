using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트의 subscribe-api에 onetime 결제를 요청할 때 입력할 정보
    /// Name, BuyerName, PhoneNumber, Email, Address, PostCode을 입력하지 않아도 결제는 되지만
    /// 실제 결제 기록에 제목이나 이름이 남지 않습니다.
    /// 특히 Name, BuyerName, (PhoneNumber 또는 Email)은 가능한 입력하는 것이 좋습니다.
    /// </summary>
    /// <seealso href="http://api.iamport.kr/#!/subscribe"/>
    public class DirectPaymentRequest
    {
        /// <summary>
        /// (required)이 결제를 거래할 때 사용할 고유 ID(OrderId 등)
        /// </summary>
        [JsonProperty("merchant_uid")]
        [Required]
        [MaxLength(80)]
        public string TransactionId { get; set; }
        /// <summary>
        /// (required)결제 총액
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
        /// (required)카드번호(dddd-dddd-dddd-dddd)
        /// </summary>
        [JsonProperty("card_number")]
        [Required]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}-\d{4}$")]
        public string CardNumber { get; set; }
        /// <summary>
        /// (required)카드 유효기간(YYYY-MM)
        /// </summary>
        [JsonProperty("expiry")]
        [Required]
        [RegularExpression(@"^\d{4}-\d{2}$")]
        public string Expiry { get; set; }
        /// <summary>
        /// (required)생년월일6자리(법인카드의 경우 사업자등록번호10자리)
        /// </summary>
        [JsonProperty("birth")]
        [Required]
        [RegularExpression(@"^\d{6}$|^\d{10}$")]
        public string AuthenticationNumber { get; set; }
        /// <summary>
        /// 카드 비밀번호 앞 두자리(법인카드의 경우 생략)
        /// </summary>
        [JsonProperty("pwd_2digit")]
        [RegularExpression(@"^(\d{2})?$")]
        public string PartialPassword { get; set; }
        /// <summary>
        /// (optional)고객 고유번호(빌링키에 대응하는 고객 식별 문자열)
        /// iamport:
        /// string 타입의 고객 고유번호.
        /// (결제에 사용된 카드정보를 빌링키 형태로 저장해두고 재결제에 사용하시려면 customer_uid를 지정해주세요. /subscribe/payments/again, /subscribe/payments/schedule로 재결제를 진행하실 수 있습니다.)
        /// </summary>
        [JsonProperty("customer_uid ")]
        [MaxLength(80)]
        public string CustomerId { get; set; }
        /// <summary>
        /// (optional)카드할부개월수. 2 이상의 integer 할부개월수 적용(결제금액 50,000원 이상 한정)
        /// </summary>
        [JsonProperty("card_quota")]
        public int InstallmentMonths { get; set; }
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
