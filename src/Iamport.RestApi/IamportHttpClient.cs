using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Microsoft.Extensions.OptionsModel;
using System.Threading.Tasks;
using Iamport.RestApi.Models;
using Newtonsoft.Json;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트 서버와의 통신을 담당하는 HTTP 클라이언트 클래스.
    /// </summary>
    public class IamportHttpClient : IIamportHttpClient, IDisposable
    {
        private const string UsersGetTokenPath = "/users/getToken";
        private readonly IamportHttpClientOptions options;
        private readonly HttpClient httpClient;
        private long disposalCounter;

        /// <summary>
        /// 응답이 성공임을 나타내는 코드
        /// </summary>
        public const int ResponseSuccessCode = 0;

        /// <summary>
        /// 주어진 파라미터로 Iamport.RestApi.IamportHttpClient의 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="optionsAccessor">아임포트 클라이언트의 옵션 정보 제공자.</param>
        public IamportHttpClient(IOptions<IamportHttpClientOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            options = optionsAccessor.Value;
            ValidateOptions();

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            options.HttpClientConfigure?.Invoke(httpClient);
        }

        /// <summary>
        /// 현재 사용중인 토큰 정보
        /// </summary>
        private IamportToken CurrentToken { get; set; }
        private readonly object tokenLock = new object();

        /// <summary>
        /// 주어진 정보로 아임포트 서버에 요청을 전송하고 결과를 반환합니다.
        /// 만약 요청 정보에 RequireAuthorization이 true일 경우
        /// 자동으로 Authorize 메서드를 호출합니다.
        /// </summary>
        /// <typeparam name="TRequest">요청할 콘텐트의 타입</typeparam>
        /// <typeparam name="TResult">응답받을 콘텐트의 타입</typeparam>
        /// <param name="request">요청 정보</param>
        /// <returns>응답 정보</returns>
        public async Task<IamportResponse<TResult>> RequestAsync<TRequest, TResult>(IamportRequest<TRequest> request)
        {
            ThrowsIfDisposed();
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.RequireAuthorization)
            {
                await EnsureAuthorizedAsync();
            }

            var httpRequest = GetHttpRequest(request);
            return await RequestAsync<TResult>(httpRequest);
        }

        /// <summary>
        /// 주어진 HttpRequestMessage를 아임포트 서버에 전송하고 결과를 반환합니다.
        /// </summary>
        /// <remarks>
        /// 이 메서드는 보안 토큰이 확보되어 있는지 여부를 확인하지 않고
        /// 요청을 전송합니다.
        /// 따라서 보안 토큰이 필요한 API인 경우 수동으로 Authorize 메서드를 호출해야 합니다.
        /// </remarks>
        /// <typeparam name="TResult">응답받을 콘텐트의 타입</typeparam>
        /// <param name="request">요청 정보</param>
        /// <returns>응답 정보</returns>
        public virtual async Task<IamportResponse<TResult>> RequestAsync<TResult>(HttpRequestMessage request)
        {
            ThrowsIfDisposed();
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await httpClient.SendAsync(request);
            var iamportResponse = await ParseResponseAsync<TResult>(response);
            if (iamportResponse.Code != 0)
            {
                throw new IamportResponseException(iamportResponse.Code, iamportResponse.Message);
            }
            return iamportResponse;
        }

        /// <summary>
        /// 현재 설정으로 유효한 토큰을 확보합니다.
        /// </summary>
        /// <returns>토큰 정보</returns>
        public virtual async Task<IamportToken> AuthorizeAsync()
        {
            ThrowsIfDisposed();
            var input = new IamportTokenRequest
            {
                ApiKey = options.ApiKey,
                ApiSecret = options.ApiSecret
            };

            // TODO: 인증 코드는 UsersApi와 중복된 부분임.
            // 별도로 빼는 방법이 있을지?
            var url = ApiPathUtility.Build(options.BaseUrl, UsersGetTokenPath);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Content = new JsonContent(input);
            var response = await RequestAsync<IamportToken>(httpRequest);
            if (response.Code != 0)
            {
                throw new IamportResponseException(response.Code, response.Message);
            }

            var token = response.Content;
            lock (tokenLock)
            {
                httpClient.DefaultRequestHeaders.Remove(options.AuthorizationHeaderName);
                httpClient.DefaultRequestHeaders.Add(options.AuthorizationHeaderName, token.AccessToken);
                CurrentToken = token;
            }
            return token;
        }

        /// <summary>
        /// 주어진 아임포트 요청 정보에 해당하는 HttpRequestMessage를 반환합니다.
        /// </summary>
        /// <typeparam name="TRequest">요청 정보의 타입</typeparam>
        /// <param name="request">요청 정보</param>
        /// <returns>HttpRequestMessage</returns>
        protected virtual HttpRequestMessage GetHttpRequest<TRequest>(IamportRequest<TRequest> request)
        {
            var url = ApiPathUtility.Build(options.BaseUrl, request.ApiPathAndQueryString);
            var httpRequest = new HttpRequestMessage(request.Method, url);
            if (request.Method != HttpMethod.Get)
            {
                httpRequest.Content = new JsonContent(request.Content);
            }
            return httpRequest;
        }

        /// <summary>
        /// 주어진 응답의 유효성을 검사하고 아임포트 응답결과로 파싱합니다.
        /// </summary>
        /// <typeparam name="T">결과 타입</typeparam>
        /// <param name="response">응답</param>
        /// <returns>응답결과</returns>
        /// <exception cref="UnauthorizedAccessException">토큰 정보가 없거나 허가되지 않았습니다.</exception>
        /// <exception cref="HttpRequestException">요청이 실패하였거나 응답 결과가 성공이 아닙니다.</exception>
        protected virtual async Task<IamportResponse<T>> ParseResponseAsync<T>(HttpResponseMessage response)
        {
            ThrowsIfDisposed();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
            if (response.StatusCode != System.Net.HttpStatusCode.OK
                && response.StatusCode != System.Net.HttpStatusCode.NoContent
                && response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new HttpRequestException($"Request failed with status code of {response.StatusCode}.");
            }
            var result = JsonConvert
                .DeserializeObject<IamportResponse<T>>(
                    await response.Content.ReadAsStringAsync())
                    ?? new IamportResponse<T>();
            result.HttpStatusCode = response.StatusCode;
            return result;
        }

        /// <summary>
        /// 현재 옵션이 유효한지 확인합니다.
        /// </summary>
        private void ValidateOptions()
        {
            if (string.IsNullOrEmpty(options.ImportId))
            {
                throw new ArgumentNullException(nameof(options.ImportId));
            }
            if (string.IsNullOrEmpty(options.ApiKey))
            {
                throw new ArgumentNullException(nameof(options.ApiKey));
            }
            if (string.IsNullOrEmpty(options.ApiSecret))
            {
                throw new ArgumentNullException(nameof(options.ApiSecret));
            }
            if (string.IsNullOrEmpty(options.AuthorizationHeaderName))
            {
                throw new ArgumentNullException(nameof(options.AuthorizationHeaderName));
            }
            if (string.IsNullOrEmpty(options.BaseUrl))
            {
                throw new ArgumentNullException(nameof(options.BaseUrl));
            }
        }

        /// <summary>
        /// 토큰의 유효성을 확인하고 유효하지 않을 경우 재인증합니다.
        /// 요청 지연시간을 고려하여 만료시간보다 1분 빨리 재인증합니다.
        /// </summary>
        /// <returns>Async Task</returns>
        private async Task EnsureAuthorizedAsync()
        {
            ThrowsIfDisposed();
            if (CurrentToken == null
                || CurrentToken.ExpiredAt.AddMinutes(-1) < DateTime.UtcNow
                || httpClient.DefaultRequestHeaders.Contains(options.AuthorizationHeaderName) == false)
            {
                await AuthorizeAsync();
            }
        }

        /// <summary>
        /// 현재 인스턴스가 Dispose되었는지 여부를 반환합니다.
        /// </summary>
        public bool IsDisposed { get { return Interlocked.Read(ref disposalCounter) > 0; } }

        private void ThrowsIfDisposed()
        {
            if (IsDisposed)
            {
                throw new InvalidOperationException("The instance is alread disposed.");
            }
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposalCounter, 1, 0) == 0)
            {
                httpClient.Dispose();
            }
        }
    }
}
