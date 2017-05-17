using System;
using System.ComponentModel.DataAnnotations;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트에서 허용하는 신용카드 유효기간 형식인지 여부를 검사합니다.
    /// </summary>
    public class CreditCardExpiryAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// 기본 아임포트 유효기간 번호 형식으로 초기화합니다.
        /// </summary>
        public CreditCardExpiryAttribute() : base(@"^\d{4}-\d{2}$")
        {
        }

        /// <inheritDocs />
        public override bool IsValid(object value)
        {
            if (base.IsValid(value))
            {
                if (string.IsNullOrEmpty(value?.ToString()))
                {
                    return true;
                }
                int.TryParse((value.ToString()).Substring(0, 4), out int year);
                int.TryParse((value.ToString()).Substring(5, 2), out int month);
                var expiry = new DateTimeOffset(year, month + 1, 1,0, 0, 0, TimeSpan.Zero);
                return expiry > DateTimeOffset.Now;
            }
            return false;
        }
    }
}
