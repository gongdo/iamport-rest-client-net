using Iamport.RestApi.Models;
using System.Threading.Tasks;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트 클라이언트가 구현해야 할 인터페이스입니다.
    /// </summary>
    public interface IIamportClient
    {
        /// <summary>
        /// 현재 클라이언트가 인증된 토큰을 확보하고 있는지 여부를 반환합니다.
        /// </summary>
        bool IsAuthorized { get; }
        /// <summary>
        /// 주어진 정보로 아임포트 서버에 요청을 전송하고 결과를 반환합니다.
        /// 만약 요청 정보에 RequireAuthorization이 true일 경우
        /// 자동으로 Authorize 메서드를 호출합니다.
        /// </summary>
        /// <typeparam name="TRequest">요청할 콘텐트의 타입</typeparam>
        /// <typeparam name="TResult">응답받을 콘텐트의 타입</typeparam>
        /// <param name="request">요청 정보</param>
        /// <returns>응답 정보</returns>
        Task<IamportResponse<TResult>> RequestAsync<TRequest, TResult>(IamportRequest<TRequest> request);
        /// <summary>
        /// 현재 설정으로 유효한 토큰을 확보합니다.
        /// </summary>
        /// <returns>토큰 정보</returns>
        Task<IamportToken> AuthorizeAsync();
    }
}
