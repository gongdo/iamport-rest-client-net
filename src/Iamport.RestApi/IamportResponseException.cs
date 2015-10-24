using System;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트의 API 응답 결과중 오류를 나타내는 예외입니다.
    /// </summary>
    public class IamportResponseException : Exception
    {
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
