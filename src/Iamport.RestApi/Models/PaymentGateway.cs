using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 지원하는 PG사
    /// </summary>
    public class PaymentGateway
    {
        /// <summary>
        /// KG이니시스
        /// </summary>
        [Display(Name = "KG이니시스")]
        public const string Inicis = "inicis";
        /// <summary>
        /// KG이니시스 웹표준결제
        /// </summary>
        [Display(Name = "KG이니시스(웹표준결제)")]
        public const string InicisHtml5 = "html5_inicis";
        /// <summary>
        /// 나이스정보통신
        /// </summary>
        [Display(Name = "나이스정보통신")]
        public const string Nice = "nice";
        /// <summary>
        /// LG유플러스
        /// </summary>
        [Display(Name = "LG유플러스")]
        public const string UPlus = "uplus";
        /// <summary>
        /// JTNet
        /// </summary>
        [Display(Name = "JTNet")]
        public const string JTNet = "jtnet";
        /// <summary>
        /// 카카오페이
        /// </summary>
        [Display(Name = "카카오페이")]
        public const string Kakao = "kakao";
        /// <summary>
        /// 다날(휴대폰 소액결제)
        /// </summary>
        [Display(Name = "다날(휴대폰 소액결제)")]
        public const string Danal = "danal";
        /// <summary>
        /// 다날(휴대폰 소액결제)
        /// </summary>
        [Display(Name = "다날(신용카드/계좌이체/가상계좌)")]
        public const string DanalTPay = "danal_tpay";
        /// <summary>
        /// 페이팔-ExpressCheckout
        /// </summary>
        [Display(Name = "페이팔-ExpressCheckout")]
        public const string PayPal = "paypal";
        /// <summary>
        /// NHN KCP(한국 사이버 결제)
        /// </summary>
        [Display(Name = "NHN KCP")]
        public const string Kcp = "kcp";
        /// <summary>
        /// Payco
        /// </summary>
        [Display(Name = "페이코")]
        public const string Payco = "payco";
        /// <summary>
        /// 네이버페이
        /// </summary>
        [Display(Name = "네이버페이")]
        public const string NaverPay = "naverco";
        /// <summary>
        /// 한국정보통신
        /// </summary>
        [Display(Name = "한국정보통신")]
        public const string Kicc = "kicc";
    }
}
