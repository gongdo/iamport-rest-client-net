using Iamport.RestApi.Models;
using Newtonsoft.Json;

namespace Sample.AspNetCore.ViewModels
{
    /// <summary>
    /// 결제 요청에 필요한 정보
    /// </summary>
    public class RequestPaymentViewModel
    {
        public static RequestPaymentViewModel Create(
            Models.Payment payment, 
            string iamportId, 
            string notificationUrl, 
            string returnUrl, 
            string appScheme)
        {
            var request = new PaymentRequest
            {
                //MerchantId = "PG사에 등록한 상점 ID가 여러개인 경우 설정",
                AppScheme = appScheme,
                NotificationUrl = notificationUrl,
                ReturnUrl = returnUrl,
                Amount = (int)payment.Amount,
                PaymentGateway = payment.PaymentGateway,
                Method = payment.PaymentMethod,
                IsDigital = payment.IsDigital,
                CustomerName = payment.CustomerName,
                CustomerPhoneNumber = payment.CustomerPhoneNumber,
                CustomerEmail = payment.CustomerEmail,
                Title = payment.Name,
                TransactionId = payment.TransactionId,
                UseEscrow = payment.UseEscrow,
                VirtualBankExpiration = payment.VirtualBankExpirationTime,
                // 다음 정보는 필요에 따라 설정합니다.
                //Currency = null,
                //CustomData = null,
                //CustomerName = null,
                //CustomerAddress = null,
                //CustomerPostCode = null,
            };
            return new RequestPaymentViewModel
            {
                IamportId = iamportId,
                ReturnUrl = returnUrl,
                Request = request,
            };
        }

        /// <summary>
        /// 아임포트 가맹점 식별 코드
        /// </summary>
        public string IamportId { get; set; }
        /// <summary>
        /// 결제처리 후 돌아갈 URL;
        /// 이 URL은 해당 결제의 진행 상태를 확인할 수 있는 페이지이어야 합니다.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 아임포트에 전송할 요청 정보
        /// </summary>
        public PaymentRequest Request { get; set; }

        /// <summary>
        /// 아임포트에 전송할 요청 정보를 Json으로 직렬화한 문자열을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public string GetRequestJson()
            => JsonConvert.SerializeObject(Request);
    }
}
