using System;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트의 API 응답 결과중 오류를 나타내는 예외입니다.
    /// </summary>
    public class IamportResponseException : Exception
    {
        /// <summary>
        /// 주어진 오류 코드와 메시지로 예외를 초기화합니다.
        /// </summary>
        /// <param name="code">오류 코드</param>
        /// <param name="message">오류 메시지</param>
        public IamportResponseException(int code, string message) : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// 내부 오류 코드.
        /// </summary>
        public int Code { get; set; }
    }
}
