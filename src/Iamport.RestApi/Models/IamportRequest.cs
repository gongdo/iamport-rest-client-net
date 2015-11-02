using System.Net.Http;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트에서 API 요청시 전달할 콘텐트 및 옵션 정보를 정의하는 클래스.
    /// </summary>
    /// <typeparam name="T">콘텐트의 타입</typeparam>
    public class IamportRequest<T>
    {
        /// <summary>
        /// 호출할 API의 경로 및 쿼리스트링 문자열.
        /// API의 경로는 IamportHttpClientOptions.BaseUrl을 기준으로 조합됩니다.
        /// </summary>
        public string ApiPathAndQueryString { get; set; }
        /// <summary>
        /// HTTP Method.
        /// </summary>
        public HttpMethod Method { get; set; } = HttpMethod.Post;
        /// <summary>
        /// 사용자 권한 인증이 필요한지 여부.
        /// true일 경우 자동으로 인증 토큰을 받은 후 진행합니다.
        /// false일 경우 인증 토큰 발급 없이 진행합니다.
        /// </summary>
        public bool RequireAuthorization { get; set; } = true;
        /// <summary>
        /// 전송할 콘텐트.
        /// 만약 Get일 경우 querystring 형식으로 전달하며
        /// 그렇지 않을 경우 json 형식으로 전달합니다.
        /// </summary>
        public virtual T Content { get; set; }
    }
    /// <summary>
    /// 아임포트에서 API 요청시 콘텐트가 없는 정보를 정의하는 클래스.
    /// </summary>
    public class IamportRequest : IamportRequest<object>
    {
        public override object Content { get { return null; } set { } }
    }
}
