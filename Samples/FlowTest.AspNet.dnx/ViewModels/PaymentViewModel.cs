using Iamport.RestApi.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace FlowTest.AspNet.dnx.ViewModels
{
    public class PaymentViewModel
    {
        /// <summary>
        /// 지원 PG사
        /// </summary>
        [Display(Name = "지원 PG사")]
        public PaymentGateway PaymentGateway { get; set; }
        /// <summary>
        /// 결제 수단
        /// </summary>
        [Display(Name = "결제 수단")]
        public PaymentMethod Method { get; set; }
        /// <summary>
        /// 에스크로 사용 여부
        /// </summary>
        [Display(Name = "에스크로 사용 여부")]
        public bool UseEscrow { get; set; }
        /// <summary>
        /// 주문이름(상품명)
        /// </summary>
        [Required]
        [MaxLength(80)]
        [Display(Name = "주문이름(결제명)")]
        public string Title { get; set; } = "테스트 상품";
        /// <summary>
        /// 결제 총액
        /// </summary>
        [Required]
        [Range(1000, 10000000)]
        [Display(Name = "금액")]
        public int Amount { get; set; } = 1004;
        /// <summary>
        /// 구매자 이름(문화상품권의 경우 UserId)
        /// </summary>
        [Required]
        [MaxLength(30)]
        [Display(Name = "구매자 이름")]
        public string CustomerName { get; set; } = "구매자";
        /// <summary>
        /// 구매자 이메일
        /// </summary>
        [Required]
        [MaxLength(255)]
        [Display(Name = "구매자 이메일")]
        public string CustomerEmail { get; set; } = "customer@email.fake";
        /// <summary>
        /// 구매자 전화번호
        /// </summary>
        [Required]
        [MaxLength(16)]
        [Display(Name = "구매자 전화번호")]
        public string CustomerPhoneNumber { get; set; } = "00012349876";
        /// <summary>
        /// 구매자 배송 주소
        /// </summary>
        [MaxLength(255)]
        [Display(Name = "구매자 주소")]
        public string CustomerAddress { get; set; }
        /// <summary>
        /// 구매자 배송 주소의 우편번호
        /// </summary>
        [MaxLength(8)]
        [Display(Name = "구매자 우편번호")]
        public string CustomerPostCode { get; set; }
        /// <summary>
        /// 가상계좌(VirtualBank)일 경우 입금 기한.
        /// 입력 포맷은 "YYYYMMDD"
        /// 입력하지 않을 경우 +2일
        /// </summary>
        [Display(Name = "<가상계좌> 입금일자")]
        [MaxLength(8)]
        public string VirtualBankExpiration { get; set; }
            = DateTimeOffset.UtcNow.AddDays(2).ToString("yyyyMMdd");
        /// <summary>
        /// 모바일 결제시 결제 앱 연동할 경우 다시 돌아올 앱의 스키마.
        /// ISP, 가상계좌, 직접입금 등에 사용합니다.
        /// </summary>
        [Display(Name = "<모바일> 앱 scheme")]
        public string AppScheme { get; set; }
    }
}
