using Iamport.RestApi.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sample.AspNetCore.ViewModels
{
    /// <summary>
    /// 구매 정보 등록 모델.
    /// 편의상 샘플 데이터로 초기화 합니다.
    /// </summary>
    public class RegisterCheckoutModel
    {
        [Required]
        [MaxLength(60)]
        [Display(Name = "구매 상품 이름")]
        public string Name { get; set; } = "상품";
        [Required]
        [Range(1000, 99999999)]
        [Display(Name = "구매 금액")]
        public decimal Amount { get; set; } = 1004;
        [Display(Name = "디지털 콘텐츠인지 여부")]
        public bool IsDigital { get; set; } = true;

        [Required]
        [Display(Name = "결제 게이트웨이사")]
        public PaymentGateway PaymentGateway { get; set; } = PaymentGateway.InicisHtml5;
        [Required]
        [Display(Name = "결제 수단")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;
        [Display(Name = "에스크로 사용 여부")]
        public bool UseEscrow { get; set; } = false;
        [Required]
        [MaxLength(20)]
        [Display(Name = "구매자 이름")]
        public string CustomerName { get; set; } = "고객이름";
        [Required]
        [MaxLength(20)]
        [Display(Name = "구매자 전화번호")]
        public string CustomerPhoneNumber { get; set; } = "01000000000";
        [MaxLength(256)]
        [Display(Name = "구매자 이메일")]
        public string CustomerEmail { get; set; } = "test@test.fake";
        [Display(Name = "가상계좌 입금기한")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTimeOffset VirtualBankExpirationTime { get; set; }
            = DateTimeOffset.UtcNow.AddDays(2).Date;
        [MaxLength(1024)]
        [Display(Name = "결제후 돌아올 URL")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 새 구매 정보 엔터티를 반환합니다.
        /// </summary>
        /// <returns>새 구매 정보 엔터티의 인스턴스</returns>
        public Models.Checkout ToCheckout()
        {
            return Models.Checkout.Create(
                Name,
                Amount,
                PaymentGateway,
                PaymentMethod,
                UseEscrow,
                CustomerName,
                CustomerPhoneNumber,
                CustomerEmail,
                VirtualBankExpirationTime,
                IsDigital,
                ReturnUrl);
        }
    }
}
