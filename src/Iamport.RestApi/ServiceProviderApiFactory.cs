using System;
using Microsoft.Framework.DependencyInjection;

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
        /// 주어진 타입의 API 인스턴스를 반환합니다.
        /// </summary>
        /// <typeparam name="T">API 클래스의 타입</typeparam>
        /// <returns>API 인스턴스</returns>
        public virtual T GetApi<T>() where T : class, IIamportApi
        {
            return serviceProvider.GetService<T>();
        }
    }
}
