using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// /payments/prepare API로 결제 준비할 내용을 정의하는 클래스입니다.
    /// 결제할 거래 ID와 금액이 고정되며, 준비된 거래는 단 한번만 결제할 수 있습니다.
    /// 현재는 한화(KRW)만을 대상으로 합니다.
    /// </summary>
    public class PaymentPreparation
    {
        /// <summary>
        /// 이 결제를 거래할 때 사용할 고유 ID(OrderId 또는 MerchantId 등)
        /// </summary>
        [Required]
        [MaxLength(80)]
        [JsonProperty("merchant_uid")]
        public string TransactionId { get; set; }
        /// <summary>
        /// 결제 총액(KRW)
        /// </summary>
        [Required]
        [Range(1000, 10000000)]
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
