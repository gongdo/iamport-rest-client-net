using Iamport.RestApi.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sample.AspNetCore.ViewModels
{
    /// <summary>
    /// 결제 요청 뷰모델.
    /// 편의상 샘플 데이터로 초기화 합니다.
    /// </summary>
    public class RequestPaymentModel
    {
        [Required]
        [MaxLength(60)]
        public string Name { get; set; } = "상품";
        [Required]
        [Range(1000, 99999999)]
        public decimal Amount { get; set; } = 1004;
        public bool IsDigital { get; set; } = true;
        [Required]
        public PaymentGateway PaymentGateway { get; set; } = PaymentGateway.InicisHtml5;
        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;
        public bool UseEscrow { get; set; } = false;
        [Required]
        [MaxLength(20)]
        public string CustomerName { get; set; } = "고객이름";
        [Required]
        [MaxLength(20)]
        public string CustomerPhoneNumber { get; set; } = "01000000000";
        [MaxLength(256)]
        public string CustomerEmail { get; set; } = "test@test.fake";
        public DateTimeOffset VirtualBankExpirationTime { get; set; }
            = DateTimeOffset.UtcNow.AddDays(2).Date;
        [MaxLength(1024)]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 새 결제 엔터티를 반환합니다.
        /// </summary>
        /// <returns>새 결제 엔터티의 인스턴스</returns>
        public Models.Payment ToPayment()
        {
            return Models.Payment.Create(
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
