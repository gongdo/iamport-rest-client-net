using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 결제 조회 결과를 페이징할 때 입력할 정보를 나타내는 클래스입니다.
    /// </summary>
    public class PaymentPageQuery : PageQuery
    {
        /// <summary>
        /// 조회할 결제 결과의 상태
        /// </summary>
        [DataMember(Name = "payment_status")]
        [JsonProperty(PropertyName = "payment_status")]
        public PaymentQueryState State { get; set; }
    }
}
