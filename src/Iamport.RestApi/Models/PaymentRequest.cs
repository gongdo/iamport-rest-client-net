using Iamport.RestApi.JsonConverters;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 결제를 요청하는 정보를 정의하는 클래스입니다.
    /// request_pay() 메서드의 파라미터를 나타냅니다.
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// 지원 PG사
        /// </summary>
        [JsonProperty("pg")]
        public PaymentGateway PaymentGateway { get; set; }
        /// <summary>
        /// 결제 수단
        /// </summary>
        [JsonProperty("pay_method")]
        public PaymentMethod Method { get; set; }
        /// <summary>
        /// 에스크로 사용 여부
        /// </summary>
        [JsonProperty("escrow", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public bool UseEscrow { get; set; }
        /// <summary>
        /// 이 결제를 거래할 때 사용할 고유 ID(OrderId 또는 MerchantId 등)
        /// </summary>
        [JsonProperty("merchant_uid")]
        [Required]
        [MaxLength(80)]
        public string TransactionId { get; set; }
        /// <summary>
        /// 주문이름
        /// </summary>
        [JsonProperty("name")]
        [Required]
        [MaxLength(80)]
        public string Title { get; set; }
        /// <summary>
        /// 결제 총액
        /// </summary>
        [JsonProperty("amount")]
        [Required]
        [Range(1000, 10000000)]
        public int Amount { get; set; }
        /// <summary>
        /// 구매자 이름(문화상품권의 경우 UserId)
        /// </summary>
        [JsonProperty("buyer_name")]
        [Required]
        [MaxLength(30)]
        public string CustomerName { get; set; }
        /// <summary>
        /// 구매자 이메일
        /// </summary>
        [JsonProperty("buyer_email")]
        [Required]
        [MaxLength(255)]
        public string CustomerEmail { get; set; }
        /// <summary>
        /// 구매자 전화번호
        /// </summary>
        [JsonProperty("buyer_tel")]
        [Required]
        [MaxLength(16)]
        public string CustomerPhoneNumber { get; set; }
        /// <summary>
        /// 구매자 배송 주소
        /// </summary>
        [JsonProperty("buyer_addr", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        [MaxLength(255)]
        public string CustomerAddress { get; set; }
        /// <summary>
        /// 구매자 배송 주소의 우편번호
        /// </summary>
        [JsonProperty("buyer_postcode", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        [MaxLength(8)]
        public string CustomerPostCode { get; set; }
        /// <summary>
        /// 커스텀 데이터
        /// </summary>
        [JsonProperty("custom_data", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public object CustomData { get; set; }
        /// <summary>
        /// 가상계좌(VirtualBank)일 경우 입금 기한.
        /// 입력하지 않을 경우 +2일
        /// </summary>
        [JsonProperty("vbank_due", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DateTimeOffsetJsonConverter))]
        public DateTimeOffset? VirtualBankExpiration { get; set; } = DateTimeOffset.UtcNow.AddDays(2);
        /// <summary>
        /// 모바일 결제시 결제 성공후 돌아올 페이지의 URL.
        /// </summary>
        [JsonProperty("m_redirect_url", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 모바일 결제시 결제 앱 연동할 경우 다시 돌아올 앱의 스키마.
        /// ISP, 가상계좌, 직접입금 등에 사용합니다.
        /// </summary>
        [JsonProperty("app_scheme", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string AppScheme { get; set; }
        /// <summary>
        /// 결제 요청에 대해 콜백 알림을 받을 URL
        /// </summary>
        [JsonProperty("notice_url", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string NotificationUrl { get; set; }
    }
}
