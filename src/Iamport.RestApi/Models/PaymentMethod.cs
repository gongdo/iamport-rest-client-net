using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 지불수단
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentMethod
    {
        /// <summary>
        /// 신용카드(ISP, 안심클릭, 국민앱카드, 케이페이 등)
        /// </summary>
        [EnumMember(Value = "card")]
        CreditCard,
        /// <summary>
        /// 실시간 계좌이체
        /// </summary>
        [EnumMember(Value = "trans")]
        Transfer,
        /// <summary>
        /// 가상 계좌
        /// </summary>
        [EnumMember(Value = "vbank")]
        VirtualBank,
        /// <summary>
        /// 모바일폰
        /// </summary>
        [EnumMember(Value = "phone")]
        Mobile,
    }
}
