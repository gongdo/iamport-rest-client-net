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
        public string ImportId { get; set; }
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
        /// HttpClient 추가 설정 액션입니다.
        /// HttpClient의 기본 설정을 변경할 때 사용합니다.
        /// null일 경우 무시하며 그렇지 않을 경우 
        /// 기본 설정이 완료된 HttpClient의 인스턴스를 파라미터로 전달합니다.
        /// </summary>
        public Action<HttpClient> HttpClientConfigure { get; set; }
    }
}
