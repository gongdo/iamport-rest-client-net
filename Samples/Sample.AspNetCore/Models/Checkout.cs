using Iamport.RestApi.Models;
using System;

namespace Sample.AspNetCore.Models
{
    /// <summary>
    /// 구매 정보 엔터티
    /// </summary>
    public class Checkout
    {
        public static Checkout Create(
            string name,
            decimal amount,
            string paymentGateway,
            string paymentMethod,
            bool useEscrow,
            string customerName,
            string customerPhoneNumber,
            string customerEmail,
            DateTimeOffset virtualBankExpirationTime,
            bool isDigital,
            string returnUrl
            )
        {
            return new Checkout
            {
                Id = Guid.NewGuid(),
                CreatedTime = DateTimeOffset.UtcNow,
                Name = name,
                Amount = amount,
                PaymentGateway = paymentGateway,
                PaymentMethod = paymentMethod,
                UseEscrow = useEscrow,
                CustomerName = customerName,
                CustomerPhoneNumber = customerPhoneNumber,
                CustomerEmail = customerEmail,
                VirtualBankExpirationTime = virtualBankExpirationTime,
                IsDigital = isDigital,
                ReturnUrl = returnUrl,
            };
        }

        /// <summary>
        /// 구매 정보 ID
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// 결제 요청 시각
        /// </summary>
        public DateTimeOffset CreatedTime { get; private set; }

        /*
         * 다음은 결제 시작시 사용자로부터 획득한 정보입니다.
         * 보통 결제사나 에스크로 유무는 사용자가 결정하지 않지만 
         * 이 애플리케이션에서는 예를 보이기 위해 포함하였습니다.
         */

        /// <summary>
        /// 주문명
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 주문액 합계
        /// </summary>
        public decimal Amount { get; private set; }
        /// <summary>
        /// 결제에 사용할 PG사
        /// </summary>
        public string PaymentGateway { get; private set; }
        /// <summary>
        /// 결제 수단
        /// </summary>
        public string PaymentMethod { get; private set; }
        /// <summary>
        /// 에스크로 사용 여부
        /// </summary>
        public bool UseEscrow { get; private set; }
        /// <summary>
        /// 고객 이름(Iamport 사용시 필수)
        /// </summary>
        public string CustomerName { get; private set; }
        /// <summary>
        /// 고객 전화번호(대부분의 PG에서 필수)
        /// </summary>
        public string CustomerPhoneNumber { get; private set; }
        /// <summary>
        /// 고객 이메일(설정하면 해당 이메일로 PG사에서도 결과를 전송)
        /// </summary>
        public string CustomerEmail { get; private set; }
        /// <summary>
        /// 결제 수단이 가상 계좌일 때 만료 일시
        /// </summary>
        public DateTimeOffset VirtualBankExpirationTime { get; private set; }
        /// <summary>
        /// 디지털 상품인지 여부. 휴대폰 결제일 때 반드시 확인 필요.
        /// </summary>
        public bool IsDigital { get; private set; }
        /// <summary>
        /// 결제 완료후 돌아갈 URL.
        /// PC 웹 브라우저에서는 결제 완료후 스크립트에서 결과를 처리할 기회가 있지만
        /// 모바일 웹 브라우저 및 WebView에서는 결제 결과를 처리할 수 없고
        /// 곧바로 지정한 ReturnUrl로 돌아갑니다.
        /// 만약 이 URL을 지정하지 않을 경우 자체 샘플 결과 페이지를 보여줍니다.
        /// </summary>
        public string ReturnUrl { get; private set; }
    }
}
