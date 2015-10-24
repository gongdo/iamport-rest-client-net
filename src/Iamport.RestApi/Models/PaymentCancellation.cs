using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 결제를 취소할 때 입력할 정보를 정의하는 클래스입니다.
    /// </summary>
    public class PaymentCancellation
    {
        /// <summary>
        /// 아임포트 결제 고유 ID
        /// </summary>
        [JsonProperty("imp_uid")]
        public string IamportId { get; set; }
        /// <summary>
        /// 가맹점에서 전달한 거래 고유 ID.
        /// mp_uid, merchant_uid 중 하나는 필수이어야 합니다.
        /// 두 값이 모두 넘어오면 imp_uid를 우선 적용합니다.
        /// </summary>
        [JsonProperty("merchant_uid")]
        public string TransactionId { get; set; }
        /// <summary>
        /// 부분취소요청금액(누락이면 전액취소)
        /// </summary>
        [JsonProperty("amount")]
        public int Amount { get; set; }
        /// <summary>
        /// 취소 사유.
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }
        /// <summary>
        /// 환불계좌 예금주(가상계좌취소시 필수)
        /// </summary>
        [JsonProperty("refund_holder")]
        public string RefundAccountHolder { get; set; }
        /// <summary>
        /// 환불계좌 은행코드(은행코드표 참조, 가상계좌취소시 필수)
        /// </summary>
        /// <seealso cref="FinancialCompanies"/>
        [JsonProperty("refund_bank")]
        [MaxLength(2)]
        public string RefundAccountBank { get; set; }
        /// <summary>
        /// 환불계좌 계좌번호(가상계좌취소시 필수)
        /// </summary>
        [JsonProperty("refund_account")]
        public string RefundAccount { get; set; }
    }
}
