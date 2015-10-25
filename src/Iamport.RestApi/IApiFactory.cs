using System;

namespace Iamport.RestApi
{
    /// <summary>
    /// API를 생산하는 팩터리가 구현해야할 인터페이스.
    /// </summary>
    public interface IApiFactory
    {
        /// <summary>
        /// 주어진 서비스 타입으로 등록된 API 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="apiServiceType">API 서비스 타입</param>
        /// <returns>등록된 API 인스턴스</returns>
        object GetApi(Type apiServiceType);
    }

    public static class IApiFactoryExteions
    {
        /// <summary>
        /// 주어진 타입의 API 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="apiFactory">API 팩토리의 인스턴스</param>
        /// <typeparam name="T">API 클래스의 타입</typeparam>
        /// <returns>API 인스턴스</returns>
        public static T GetApi<T>(this IApiFactory apiFactory) where T : class, IIamportApi
        {
            return apiFactory.GetApi(typeof(T)) as T;
        }
    }
}
