using System;
using System.ComponentModel.DataAnnotations;

namespace Sample.AspNetCore.ViewModels
{
    /// <summary>
    /// 새 결제 요청 모델
    /// </summary>
    public class RegisterPaymentModel
    {
        /// <summary>
        /// 구매 정보 Id
        /// </summary>
        [Required]
        public Guid CheckoutId { get; set; }
    }
}
