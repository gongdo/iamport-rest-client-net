using Iamport.RestApi.Extensions;
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
    /// <seealso>https://github.com/iamport/iamport-manual/blob/master/%EC%9D%B8%EC%A6%9D%EA%B2%B0%EC%A0%9C/getstarted.md#21-아임포트로-pg결제창-띄우기</seealso>
    public class PaymentRequest
    {
        /// <summary>
        /// 하나의 아임포트계정으로 여러 PG를 사용할 때 구분자
        /// </summary>
        /// <remarks>
        /// (선택항목) 누락되거나 매칭되지 않는 경우 아임포트 관리자페이지에서 설정한 "기본PG"가 호출됨
        /// "kakao", "html5_inicis"와 같이 { PG사명 }만 지정,
        /// "html5_inicis.INIpayTest"와 같이 { PG사명 }.{상점아이디}로 지정
        /// </remarks>
        [JsonProperty("pg")]
        public string PaymentGatewayOutput
        {
            get
            {
                return PaymentGateway.HasValue
                    ? PaymentGateway.Value.GetMemberValue()
                        + (string.IsNullOrEmpty(MerchantId)
                        ? ""
                        : "." + MerchantId)
                    : null;
            }
        }
        /// <summary>
        /// 알려진 PG사 명
        /// </summary>
        [JsonIgnore]
        public PaymentGateway? PaymentGateway { get; set; }
        /// <summary>
        /// 여러 상점을 관리할 때 사용할 상점 ID 구분자
        /// </summary>
        [JsonIgnore]
        public string MerchantId { get; set; }
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
        public decimal Amount { get; set; }
        /// <summary>
        /// 결제 총액의 통화(2017-02-16현재 지원 통화: KRW, USD, EUR)
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; } = "KRW";
        /// <summary>
        /// 구매자 이름(문화상품권의 경우 UserId)
        /// </summary>
        [JsonProperty("buyer_name")]
        [MaxLength(30)]
        public string CustomerName { get; set; }
        /// <summary>
        /// 구매자 이메일
        /// </summary>
        [JsonProperty("buyer_email")]
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
        /// 결제 요청에 대해 콜백 알림을 받을 URL
        /// </summary>
        [JsonProperty("notice_url", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string NotificationUrl { get; set; }

        /// <summary>
        /// 결제상품이 컨텐츠인지 여부
        /// </summary>
        /// <remarks>
        /// 휴대폰소액결제시 필수. 반드시 실물/컨텐츠를 정확히 구분해주어야 함
        /// </remarks>
        [JsonProperty("digital")]
        public bool IsDigital { get; set; }
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

    }
}
