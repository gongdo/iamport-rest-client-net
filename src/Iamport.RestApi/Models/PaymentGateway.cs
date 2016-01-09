using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 지원하는 PG사
    /// </summary>
    public enum PaymentGateway
    {
        /// <summary>
        /// KG이니시스
        /// </summary>
        [EnumMember(Value = "inicis")]
        [Display(Name = "KG이니시스")]
        Inicis,
        /// <summary>
        /// 나이스정보통신
        /// </summary>
        [EnumMember(Value = "nice")]
        [Display(Name = "나이스정보통신")]
        Nice
    }
}
