using System;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트 기능을 제공하는 클래스입니다.
    /// 기능들을 시작하는 엔트리 포인트의 역할을 수행합니다.
    /// </summary>
    public class Iamport
    {
        /// <summary>
        /// 응답이 성공임을 나타내는 코드
        /// </summary>
        public const int ResponseSuccessCode = 0;

        private readonly IApiFactory apiFactory;

        /// <summary>
        /// 주어진 API 팩토리로 초기화합니다.
        /// </summary>
        /// <param name="apiFactory"></param>
        public Iamport(IApiFactory apiFactory)
        {
            if (apiFactory == null)
            {
                throw new ArgumentNullException(nameof(apiFactory));
            }
            this.apiFactory = apiFactory;
        }

        /// <summary>
        /// 주어진 타입의 API 인스턴스를 반환합니다.
        /// </summary>
        /// <typeparam name="T">API 클래스의 타입</typeparam>
        /// <returns>API 인스턴스</returns>
        public T Api<T>() where T : class, IIamportApi
        {
            return apiFactory.GetApi<T>();
        }
    }
}
