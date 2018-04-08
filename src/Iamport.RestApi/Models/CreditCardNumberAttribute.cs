using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트에서 허용하는 신용카드 번호 형식인지 여부를 검사합니다.
    /// </summary>
    public class CreditCardNumberAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// 기본 아임포트 신용카드 번호 형식으로 초기화합니다.
        /// </summary>
        public CreditCardNumberAttribute() : base(@"^\d{4}-\d{4}-\d{4}-\d{2,4}$")
        {
        }
    }
}
