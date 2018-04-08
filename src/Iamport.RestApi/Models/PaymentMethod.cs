using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 지불수단
    /// </summary>
    public class PaymentMethod
    {
        /// <summary>
        /// 신용카드(ISP; 안심클릭; 국민앱카드; 케이페이 등)
        /// </summary>
        [Display(Name = "신용카드")]
        public const string CreditCard = "card";
        /// <summary>
        /// 실시간 계좌이체
        /// </summary>
        [Display(Name = "실시간 계좌이체")]
        public const string Transfer = "trans";
        /// <summary>
        /// 가상 계좌
        /// </summary>
        [Display(Name = "가상계좌")]
        public const string VirtualBank = "vbank";
        /// <summary>
        /// 모바일폰
        /// </summary>
        [Display(Name = "모바일")]
        public const string Mobile = "phone";
        /// <summary>
        /// 컬처랜드 문화상품권
        /// </summary>
        [Display(Name = "문화상품권")]
        public const string CultureLand = "cultureland";
        /// <summary>
        /// 스마트문상
        /// </summary>
        [Display(Name = "스마트문상")]
        public const string SmartCulture = "smartculture";
        /// <summary>
        /// 해피머니
        /// </summary>
        [Display(Name = "해피머니")]
        public const string HappyMoney = "happymoney";
    }
}
