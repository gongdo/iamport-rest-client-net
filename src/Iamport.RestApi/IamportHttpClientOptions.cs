using System;
using System.Net.Http;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트 HTTP 클라이언트 구성 옵션을 정의하는 클래스입니다.
    /// </summary>
    public class IamportHttpClientOptions
    {
        /// <summary>
        /// 아임포트 가맹점 식별 코드
        /// </summary>
        public string IamportId { get; set; }
        /// <summary>
        /// API 키
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// API 비밀번호
        /// </summary>
        public string ApiSecret { get; set; }
        /// <summary>
        /// API 기본 주소
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.iamport.kr";
        /// <summary>
        /// 인증용 HTTP 헤더 이름
        /// </summary>
        public string AuthorizationHeaderName { get; set; } = "X-ImpTokenHeader";

        /// <summary>
        /// 현재 옵션이 유효한지 확인합니다.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrEmpty(IamportId))
            {
                throw new ArgumentNullException(nameof(IamportId));
            }
            if (string.IsNullOrEmpty(ApiKey))
            {
                throw new ArgumentNullException(nameof(ApiKey));
            }
            if (string.IsNullOrEmpty(ApiSecret))
            {
                throw new ArgumentNullException(nameof(ApiSecret));
            }
            if (string.IsNullOrEmpty(AuthorizationHeaderName))
            {
                throw new ArgumentNullException(nameof(AuthorizationHeaderName));
            }
            if (string.IsNullOrEmpty(BaseUrl))
            {
                throw new ArgumentNullException(nameof(BaseUrl));
            }
        }

    }
}
