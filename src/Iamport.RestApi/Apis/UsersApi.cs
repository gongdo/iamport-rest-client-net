using System;
using System.Threading.Tasks;
using Iamport.RestApi.Models;
using System.Net.Http;

namespace Iamport.RestApi.Apis
{
    /// <summary>
    /// Users API의 기본 구현 클래스입니다.
    /// </summary>
    public class UsersApi : IUsersApi
    {
        private const string UsersGetTokenPath = "getToken";

        private readonly IIamportHttpClient client;
        public UsersApi(IIamportHttpClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            this.client = client;
        }

        public string BasePath { get; } = "/users";

        /// <summary>
        /// 주어진 아임포트 토큰 요청을 인증하고 결과 토큰을 반환합니다.
        /// 인증에 실패하거나 입력 정보에 문제가 있을 경우 예외를 발생시킵니다.
        /// 이 API 호출은 내부 HttpClient의 Authorization 헤더를 설정하지 않습니다.
        /// 단지 요청한 토큰 정보에 대한 응답을 반환할 뿐입니다.
        /// </summary>
        /// <param name="request">아임포트 토큰 요청</param>
        /// <returns>인증된 아임포트 토큰</returns>
        public virtual async Task<IamportToken> GetTokenAsync(IamportTokenRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, this.BuildPath(UsersGetTokenPath));
            httpRequest.Content = new JsonContent(request);
            var response = await client.RequestAsync<IamportToken>(httpRequest);
            if (response.Code != 0)
            {
                throw new IamportResponseException(response.Code, response.Message);
            }
            return response.Content;
        }
    }
}
