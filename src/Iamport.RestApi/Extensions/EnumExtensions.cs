using System;
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
    }
}
