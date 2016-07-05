using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 결제 상태
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentStatus
    {
        /// <summary>
        /// 미결제
        /// </summary>
        [EnumMember(Value = "ready")]
        Ready,
        /// <summary>
        /// 결제완료
        /// </summary>
        [EnumMember(Value = "paid")]
        Paid,
        /// <summary>
        /// 결제취소
        /// </summary>
        [EnumMember(Value = "cancelled")]
        Cancelled,
        /// <summary>
        /// 결제실패
        /// </summary>
        [EnumMember(Value = "failed")]
        Failed,
    }
}
