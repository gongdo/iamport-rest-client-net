using Iamport.RestApi.Models;
using System.Threading.Tasks;

namespace Iamport.RestApi.Apis
{
    /// <summary>
    /// 사용자 인증 관련 API가 구현해야할 인터페이스입니다.
    /// </summary>
    public interface IUsersApi : IIamportApi
    {
        /// <summary>
        /// 주어진 아임포트 토큰 요청을 인증하고 결과 토큰을 반환합니다.
        /// 인증에 실패하거나 입력 정보에 문제가 있을 경우 예외를 발생시킵니다.
        /// </summary>
        /// <param name="request">아임포트 토큰 요청</param>
        /// <seealso>https://api.iamport.kr/#!/authenticate/getToken</seealso>
        /// <returns>인증된 아임포트 토큰</returns>
        Task<IamportToken> GetTokenAsync(IamportTokenRequest request);
    }
}
