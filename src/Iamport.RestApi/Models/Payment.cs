using Iamport.RestApi.JsonConverters;
using Newtonsoft.Json;
using System;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 결제 상태 정보를 정의하는 클래스입니다.
    /// 아임포트 API를 통해 조회한 결제의 결과를 나타냅니다.
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// 아임포트 결제 고유 ID
        /// </summary>
        [JsonProperty("imp_uid")]
        public string IamportId { get; set; }
        /// <summary>
        /// 가맹점에서 전달한 거래 고유 ID
        /// </summary>
        [JsonProperty("merchant_uid")]
        public string TransactionId { get; set; }
        /// <summary>
        /// 결제 수단. <see cref="PaymentMethod"/>
        /// </summary>
        [JsonProperty("pay_method")]
        public string Method { get; set; } = PaymentMethod.CreditCard;
        /// <summary>
        /// 알려진 PG사 명칭. <see cref="PaymentGateway"/>
        /// </summary>
        /// <seealso></seealso>
        [JsonProperty("pg_provider")]
        public string PaymentGateway { get; set; } = Models.PaymentGateway.Inicis;
        /// <summary>
        /// PG사 승인정보
        /// </summary>
        [JsonProperty("pg_tid")]
        public string PaymentGatewayTransactionId { get; set; }
        /// <summary>
        /// 에스크로 결제 여부
        /// </summary>
        [JsonProperty("escrow ")]
        public bool Escrow { get; set; }
        /// <summary>
        /// 카드사 승인정보(계좌이체 / 가상계좌는 값 없음)
        /// </summary>
        [JsonProperty("apply_num")]
        public string ApplyNumber { get; set; }
        /// <summary>
        /// 카드사 명칭
        /// </summary>
        [JsonProperty("card_name")]
        public string CreditCardCompanyName { get; set; }
        /// <summary>
        /// 할부개월 수(0이면 일시불)
        /// </summary>
        [JsonProperty("card_quota")]
        public int InstallmentPlanPeriod { get; set; }
        /// <summary>
        /// 입금받을 가상계좌 은행명
        /// </summary>
        [JsonProperty("vbank_name")]
        public string VirtualBankName { get; set; }
        /// <summary>
        /// 입금받을 가상계좌 계좌번호
        /// </summary>
        [JsonProperty("vbank_num")]
        public string VirtualBankAccount { get; set; }
        /// <summary>
        /// 입금받을 가상계좌 예금주
        /// </summary>
        [JsonProperty("vbank_holder")]
        public string VirtualBankAccountHolder { get; set; }
        /// <summary>
        /// 입금받을 가상계좌 마감기한 UNIX timestamp
        /// </summary>
        [JsonProperty("vbank_date")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime? VirtualBankExpirationUtc { get; set; }
        /// <summary>
        /// 주문명칭
        /// </summary>
        [JsonProperty("name")]
        public string Title { get; set; }
        /// <summary>
        /// 주문(결제)금액
        /// </summary>
        /// <remarks>
        /// 실제 결제승인된 금액이나 가상계좌 입금예정 금액
        /// </remarks>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        /// <summary>
        /// 결제취소금액
        /// </summary>
        [JsonProperty("cancel_amount")]
        public decimal CancelledAmount { get; set; }
        /// <summary>
        /// 결제 통화(현재 지원: KRW, USD, EUR)
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
        /// <summary>
        /// 주문자명
        /// </summary>
        [JsonProperty("buyer_name")]
        public string BuyerName { get; set; }
        /// <summary>
        /// 주문자 Email주소
        /// </summary>
        [JsonProperty("buyer_email")]
        public string BuyerEmail { get; set; }
        /// <summary>
        /// 주문자 전화번호
        /// </summary>
        [JsonProperty("buyer_tel")]
        public string BuyerPhoneNumber { get; set; }
        /// <summary>
        /// 주문자 주소
        /// </summary>
        [JsonProperty("buyer_addr")]
        public string BuyerAddress { get; set; }
        /// <summary>
        /// 주문자 우편번호
        /// </summary>
        [JsonProperty("buyer_postcode")]
        public string BuyerPostCode { get; set; }
        /// <summary>
        /// 가맹점에서 전달한 custom data.JSON string으로 전달
        /// </summary>
        [JsonProperty("custom_data")]
        public object CustomData { get; set; }
        /// <summary>
        /// 구매자가 결제를 시작한 단말기의 UserAgent 문자열
        /// </summary>
        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }
        /// <summary>
        /// 결제상태. ready:미결제, paid: 결제완료, cancelled: 결제취소, failed: 결제실패
        /// </summary>
        [JsonProperty("status")]
        public PaymentStatus Status { get; set; }
        /// <summary>
        /// 결제완료시점 UNIX timestamp.결제완료가 아닐 경우 0
        /// </summary>
        [JsonProperty("paid_at")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime? PaidAtUtc { get; set; }
        /// <summary>
        /// 결제실패시점 UNIX timestamp.결제실패가 아닐 경우 0
        /// </summary>
        [JsonProperty("failed_at")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime? FailedAtUtc { get; set; }
        /// <summary>
        /// 결제취소시점 UNIX timestamp.결제취소가 아닐 경우 0
        /// </summary>
        [JsonProperty("cancelled_at")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime? CancelledAtUtc { get; set; }
        /// <summary>
        /// 결제실패 사유
        /// </summary>
        [JsonProperty("fail_reason")]
        public string FailedReason { get; set; }
        /// <summary>
        /// 결제취소 사유
        /// </summary>
        [JsonProperty("cancel_reason")]
        public string CancelledReason { get; set; }
        /// <summary>
        /// 신용카드 매출전표 확인 URL
        /// </summary>
        [JsonProperty("receipt_url")]
        public string ReceiptUrl { get; set; }
        /// <summary>
        /// 취소/부분취소 시 생성되는 취소 매출전표 확인 URL. 부분취소 횟수만큼 매출전표가 별도로 생성됨
        /// </summary>
        [JsonProperty("cancel_receipt_urls")]
        public string[] CancelledReceiptUrls { get; set; }
    }
}
