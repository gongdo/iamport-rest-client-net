using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트의 subscribe-api에 schedule 결제를 요청할 때 입력할 정보
    /// </summary>
    /// <seealso href="http://api.iamport.kr/#!/subscribe"/>
    /// <remarks><![CDATA[
    /// 비인증 결제요청을 미리 예약해두면 아임포트가 자동으로 해당 시간에 맞춰 결제를 진행하는 방식입니다.(/subscribe/payments/again 를 아임포트가 대신 수행하는 개념)
    /// 결제가 이뤄지고나면 Notification URL로 결제성공/실패 결과를 POST request로 보내드립니다.
    /// 
    /// 1. 기존에 빌링키가 등록된 customeruid가 존재하는 경우 해당 customer_uid와 해당되는 빌링키로 schedule정보가 예약됩니다.(카드정보 선택사항)
    /// 2. 등록된 customer_uid가 없는 경우 빌링키 신규 발급을 먼저 진행한 후 schedule정보를 예약합니다.(카드정보 필수사항)
    /// 
    /// 예약건 별로 고유의 merchant_uid를 전달해주셔야 합니다.
    /// 
    /// schedules의 상세정보(선택정보) buyer_name, buyer_email, buyer_tel, buyer_addr, buyer_postcode는 누락되는 경우에 한해,
    /// customer_uid에 해당되는 customer_name, customer_email, customer_tel, customer_addr, customer_postcode 정보로 대체됩니다.
    /// (buyer 정보는 customer_정보에 우선합니다)
    /// ]]></remarks>
    public class SchedulePaymentsRequest
    {
        /// <summary>
        /// (required)고객 고유번호(빌링키에 대응하는 고객 식별 문자열)
        /// </summary>
        [JsonProperty("customer_uid")]
        [Required]
        [MaxLength(80)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 카드정상결제여부 체크용 금액. 결제 직후 자동으로 취소됩니다. (0원으로 설정할 경우 테스트하지 않음)
        /// </summary>
        [JsonProperty("checking_amount")]
        [Range(0, 10000000)]
        public decimal CheckingAmount { get; set; }
        /// <summary>
        /// (optional)카드번호(dddd-dddd-dddd-dddd)
        /// </summary>
        [JsonProperty("card_number")]
        [CreditCardNumber]
        public string CardNumber { get; set; }
        /// <summary>
        /// (optional)카드 유효기간(YYYY-MM)
        /// </summary>
        [JsonProperty("expiry")]
        [CreditCardExpiry]
        public string Expiry { get; set; }
        /// <summary>
        /// (required)생년월일6자리(법인카드의 경우 사업자등록번호10자리)
        /// </summary>
        [JsonProperty("birth")]
        [CreditCardAuthenticationNumber]
        public string AuthenticationNumber { get; set; }
        /// <summary>
        /// 카드 비밀번호 앞 두자리(법인카드의 경우 생략)
        /// </summary>
        [JsonProperty("pwd_2digit")]
        [RegularExpression(@"^(\d{2})?$")]
        public string PartialPassword { get; set; }
        /// <summary>
        /// 결제예약 스케쥴의 목록
        /// </summary>
        [JsonProperty("schedules")]
        [Required]
        public PaymentSchedule[] Schedules { get; set; }
    }
}
