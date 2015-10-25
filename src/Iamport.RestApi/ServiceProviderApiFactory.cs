using System;

namespace Iamport.RestApi
{
    /// <summary>
    /// IServiceProvider를 사용하여 API 구현체를 생산하는 팩터리 클래스입니다.
    /// </summary>
    public class ServiceProviderApiFactory : IApiFactory
    {
        private readonly IServiceProvider serviceProvider;
        public ServiceProviderApiFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 주어진 서비스 타입으로 등록된 API 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="apiServiceType">API 서비스 타입</param>
        /// <returns>등록된 API 인스턴스</returns>
        public object GetApi(Type apiServiceType)
        {
            return serviceProvider.GetService(apiServiceType);
        }
    }
}
