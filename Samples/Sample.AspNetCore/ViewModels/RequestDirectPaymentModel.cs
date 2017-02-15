using Iamport.RestApi.Models;
using System.ComponentModel.DataAnnotations;

namespace Sample.AspNetCore.ViewModels
{
    /// <summary>
    /// 비인증 결제 요청 정보 모델.
    /// 편의상 샘플 데이터로 초기화 합니다.
    /// </summary>
    public class RequestDirectPaymentModel
    {
        [Required]
        [MaxLength(60)]
        [Display(Name = "구매 상품 이름")]
        public string Name { get; set; } = "상품";
        [Required]
        [Range(1000, 99999999)]
        [Display(Name = "구매 금액")]
        public decimal Amount { get; set; } = 1004;

        [Required]
        [Display(Name = "결제 게이트웨이사")]
        public PaymentGateway PaymentGateway { get; set; } = PaymentGateway.Nice;
        /// <summary>
        /// 결제금액 중 부가세 금액(파라메터가 누락되면 10%로 자동 계산됨)
        /// </summary>
        [Display(Name = "결제금액 중 부가세")]
        public decimal Vat { get; set; }
        /// <summary>
        /// (required)카드번호(dddd-dddd-dddd-dddd)
        /// </summary>
        [Required]
        [RegularExpression(@"^(\d{4})-(\d{4})-(\d{4})-(\d{4})$")]
        [Display(Name = "카드번호(dddd-dddd-dddd-dddd)")]
        public string CardNumber { get; set; }
        /// <summary>
        /// (required)카드 유효기간(YYYY-MM)
        /// </summary>
        [Required]
        [RegularExpression(@"^(?:(\d{4})-([0][0-9]|1[0-2]))$")]
        [Display(Name = "카드 유효기간(YYYY-MM)")]
        public string Expiry { get; set; }
        /// <summary>
        /// (required)생년월일6자리(법인카드의 경우 사업자등록번호10자리)
        /// </summary>
        [Required]
        [RegularExpression(@"^\d{6}$|^\d{10}$")]
        [Display(Name = "생년월일6자리(법인카드의 경우 사업자등록번호10자리)")]
        public string AuthenticationNumber { get; set; }
        /// <summary>
        /// 카드 비밀번호 앞 두자리(법인카드의 경우 생략)
        /// </summary>
        [RegularExpression(@"^(\d{2})?$")]
        [Display(Name = "카드 비밀번호 앞 두자리(법인카드의 경우 생략)")]
        public string PartialPassword { get; set; }

        [MaxLength(20)]
        [Display(Name = "구매자 이름")]
        public string CustomerName { get; set; } = "고객이름";
        [MaxLength(20)]
        [Display(Name = "구매자 전화번호")]
        public string CustomerPhoneNumber { get; set; } = "01000000000";
        [MaxLength(256)]
        [Display(Name = "구매자 이메일")]
        public string CustomerEmail { get; set; } = "test@test.fake";
        [MaxLength(256)]
        [Display(Name = "구매자 주소")]
        public string CustomerAddress { get; set; } = "어디국 어디도 어디시 어디구 어디동 어디어디";
        [MaxLength(6)]
        [Display(Name = "구매자 우편번호")]
        public string CustomerPostCode { get; set; } = "99999";
    }
}
