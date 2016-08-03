namespace Sample.AspNetCore.ViewModels
{
    /// <summary>
    /// 결제 요청에 필요한 정보
    /// </summary>
    public class RequestPaymentViewModel
    {
        public static RequestPaymentViewModel Create(
            Models.Checkout checkout,
            string iamportId,
            string paymentUrl)
        {
            return new RequestPaymentViewModel
            {
                Checkout = checkout,
                IamportId = iamportId,
                PaymentUrl = paymentUrl,
            };
        }

        /// <summary>
        /// 아임포트 가맹점 식별 코드
        /// </summary>
        public string IamportId { get; set; }
        /// <summary>
        /// 구매 정보
        /// </summary>
        public Models.Checkout Checkout { get; set; }
        /// <summary>
        /// 결제 정보 생성 URL
        /// </summary>
        public string PaymentUrl { get; set; }
    }
}
