using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "신용카드")]
        CreditCard,
        /// <summary>
        /// 실시간 계좌이체
        /// </summary>
        [EnumMember(Value = "trans")]
        [Display(Name = "실시간 계좌이체")]
        Transfer,
        /// <summary>
        /// 가상 계좌
        /// </summary>
        [EnumMember(Value = "vbank")]
        [Display(Name = "가상계좌")]
        VirtualBank,
        /// <summary>
        /// 모바일폰
        /// </summary>
        [EnumMember(Value = "phone")]
        [Display(Name = "모바일")]
        Mobile,
        /// <summary>
        /// 컬처랜드 문화상품권
        /// </summary>
        [EnumMember(Value = "cultureland")]
        [Display(Name = "문화상품권")]
        CultureLand,
        /// <summary>
        /// 스마트문상
        /// </summary>
        [EnumMember(Value = "smartculture")]
        [Display(Name = "스마트문상")]
        SmartCulture,
        /// <summary>
        /// 해피머니
        /// </summary>
        [EnumMember(Value = "happymoney")]
        [Display(Name = "해피머니")]
        HappyMoney,
    }
}
