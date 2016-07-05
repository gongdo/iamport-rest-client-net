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
        /// KG이니시스 웹표준결제
        /// </summary>
        [EnumMember(Value = "html5_inicis")]
        [Display(Name = "KG이니시스(웹표준결제)")]
        InicisHtml5,
        /// <summary>
        /// 나이스정보통신
        /// </summary>
        [EnumMember(Value = "nice")]
        [Display(Name = "나이스정보통신")]
        Nice,
        /// <summary>
        /// LG유플러스
        /// </summary>
        [EnumMember(Value = "uplus")]
        [Display(Name = "LG유플러스")]
        UPlus,
        /// <summary>
        /// JTNet
        /// </summary>
        [EnumMember(Value = "jtnet")]
        [Display(Name = "JTNet")]
        JTNet,
        /// <summary>
        /// 카카오페이
        /// </summary>
        [EnumMember(Value = "kakao")]
        [Display(Name = "카카오페이")]
        Kakao,
        /// <summary>
        /// 다날(휴대폰 소액결제)
        /// </summary>
        [EnumMember(Value = "danal")]
        [Display(Name = "다날(휴대폰 소액결제)")]
        Danal,
        /// <summary>
        /// 페이팔-ExpressCheckout
        /// </summary>
        [EnumMember(Value = "paypal")]
        [Display(Name = "페이팔-ExpressCheckout")]
        PayPal,
    }
}
