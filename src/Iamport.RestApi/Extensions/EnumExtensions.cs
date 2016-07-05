using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Iamport.RestApi.Extensions
{
    /// <summary>
    /// Enum에 관련된 확장 메서드
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 주어진 enum 값에 선언된 EnumMemberAttribute의 Value를 반환합니다.
        /// EnumMemberAttribute가 없을 경우 enum 값을 ToString()하여 반환합니다.
        /// </summary>
        /// <param name="enumeration">enum 값</param>
        /// <returns>EnumMemberAttribute로 정의한 Value.</returns>
        public static string GetMemberValue(this Enum enumeration)
        {
            return enumeration.GetType().GetTypeInfo()
                .DeclaredMembers
                .FirstOrDefault(m => m.Name == enumeration.ToString())
                ?.GetCustomAttribute<EnumMemberAttribute>()
                ?.Value
                ?? enumeration.ToString();
        }

        /// <summary>
        /// 주어진 enum 값에 선언된 DisplayAttribute의 Name을 반환합니다.
        /// DisplayAttribute가 없을 경우 enum 값을 ToString()하여 반환합니다.
        /// </summary>
        /// <param name="enumeration">enum 값</param>
        /// <returns>DisplayAttribute로 정의한 Value.</returns>
        public static string GetDisplayName(this Enum enumeration)
        {
            return enumeration.GetType().GetTypeInfo()
                .DeclaredMembers
                .FirstOrDefault(m => m.Name == enumeration.ToString())
                ?.GetCustomAttribute<DisplayAttribute>()
                ?.Name
                ?? enumeration.ToString();
        }

        /// <summary>
        /// 주어진 타입의 Enum에 설정된 EnumMemberAttribute의 Value에 대한 Enum값의 맵을 반환합니다.
        /// EnumMemberAttribute가 없을 경우 해당 Enum을 ToString()한 값으로 매핑합니다.
        /// </summary>
        /// <typeparam name="TEnum">Enum 타입</typeparam>
        /// <returns>MemberValue에 대한 Enum값의 맵</returns>
        public static IReadOnlyDictionary<string, TEnum> GetMemberValueEnumMap<TEnum>()
        {
            var type = typeof(TEnum);
            return type.GetFields()
                .Where(m => m.IsLiteral && !m.IsSpecialName)
                .Select(m => new { MemberValue = m.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? m.Name, Member = m })
                .ToDictionary(m => m.MemberValue, m => (TEnum)Enum.Parse(type, m.Member.Name));
        }
    }
}
