using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 신규 구매자 입력 정보
    /// </summary>
    /// <seealso href="http://api.iamport.kr/#!/subscribe.customer"/>
    public class CustomerRegistration
    {
        /// <summary>
        /// (required)고객 고유번호(빌링키에 대응하는 고객 식별 문자열)
        /// </summary>
        [JsonProperty("customer_uid ")]
        [JsonRequired]
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// (required)카드번호(dddd-dddd-dddd-dddd)
        /// </summary>
        [JsonProperty("card_number")]
        [JsonRequired]
        [Required]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}-\d{4}$")]
        public string CardNumber { get; set; }
        /// <summary>
        /// (required)카드 유효기간(YYYY-MM)
        /// </summary>
        [JsonProperty("expiry")]
        [JsonRequired]
        [Required]
        [RegularExpression(@"^\d{4}-\d{2}$")]
        public string Expiry { get; set; }
        /// <summary>
        /// (required)생년월일6자리(법인카드의 경우 사업자등록번호10자리)
        /// </summary>
        [JsonProperty("birth")]
        [JsonRequired]
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
        /// (optional)고객(카드소지자) 관리용 이름
        /// </summary>
        [JsonProperty("customer_name")]
        public string Name { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 전화번호
        /// </summary>
        [JsonProperty("customer_tel")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 이메일
        /// </summary>
        [JsonProperty("customer_email")]
        public string Email { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 주소
        /// </summary>
        [JsonProperty("customer_addr")]
        public string Address { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 우편번호
        /// </summary>
        [JsonProperty("customer_postcode")]
        public string PostCode { get; set; }
    }
}
