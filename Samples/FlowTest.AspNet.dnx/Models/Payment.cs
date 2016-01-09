using Iamport.RestApi.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace FlowTest.AspNet.dnx.Models
{
    /// <summary>
    /// 실제 DB에 저장할 결제 정보
    /// 예제에서는 단순히 PaymentViewModel과 거의 동일하게 처리합니다.
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// 결제에 대한 고유 거래 ID
        /// </summary>
        public string TransactionId { get; set; }
        /// <summary>
        /// 이 거래와 연결된 아임포트의 고유 ID
        /// </summary>
        public string IamportId { get; set; }
        /// <summary>
        /// 결제 상태
        /// </summary>
        public PaymentState State { get; set; }

        /*
        이 외에도 아임포트와 관련된 필드가 다수 있지만
        그것을 저장할지는 애플리케이션에 따라 다를 것입니다.
        예제에서는 생략합니다.
        */

        /// <summary>
        /// 지원 PG사
        /// </summary>
        public PaymentGateway PaymentGateway { get; set; }
        /// <summary>
        /// 결제 수단
        /// </summary>
        public PaymentMethod Method { get; set; }
        /// <summary>
        /// 에스크로 사용 여부
        /// </summary>
        public bool UseEscrow { get; set; }
        /// <summary>
        /// 주문이름(상품명)
        /// </summary>
        [Required]
        [MaxLength(80)]
        public string Title { get; set; }
        /// <summary>
        /// 결제 총액
        /// </summary>
        [Required]
        [Range(1000, 10000000)]
        public int Amount { get; set; }
        /// <summary>
        /// 구매자 이름(문화상품권의 경우 UserId)
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string CustomerName { get; set; }
        /// <summary>
        /// 구매자 이메일
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string CustomerEmail { get; set; }
        /// <summary>
        /// 구매자 전화번호
        /// </summary>
        [Required]
        [MaxLength(16)]
        public string CustomerPhoneNumber { get; set; }
        /// <summary>
        /// 구매자 배송 주소
        /// </summary>
        [MaxLength(255)]
        public string CustomerAddress { get; set; }
        /// <summary>
        /// 구매자 배송 주소의 우편번호
        /// </summary>
        [MaxLength(8)]
        public string CustomerPostCode { get; set; }
        /// <summary>
        /// 가상계좌(VirtualBank)일 경우 입금 기한.
        /// </summary>
        [MaxLength(8)]
        public DateTimeOffset? VirtualBankExpiration { get; set; }
        /// <summary>
        /// 모바일 결제시 결제 앱 연동할 경우 다시 돌아올 앱의 스키마.
        /// ISP, 가상계좌, 직접입금 등에 사용합니다.
        /// </summary>
        public string AppScheme { get; set; }
    }
}
