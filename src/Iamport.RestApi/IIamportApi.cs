using System;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트 API가 구현해야 할 인터페이스입니다.
    /// </summary>
    public interface IIamportApi
    {
        /// <summary>
        /// 이 API의 기본 경로
        /// </summary>
        string BasePath { get; }
    }

    /// <summary>
    /// 아임포트 API의 확장메서드
    /// </summary>
    public static class IIamportApiExtensions
    {
        /// <summary>
        /// 주어진 경로 및 쿼리스트링을 API의 기본 경로에 추가하거나 덮어씁니다.
        /// </summary>
        /// <param name="api">API</param>
        /// <param name="path">추가 경로 및 쿼리스트링</param>
        /// <returns>완성된 경로 및 쿼리스트링</returns>
        public static string BuildPath(this IIamportApi api, string path)
        {
            if (api == null)
            {
                throw new ArgumentNullException(nameof(api));
            }
            return ApiPathUtility.Build(api.BasePath, path);
        }
    }
}
