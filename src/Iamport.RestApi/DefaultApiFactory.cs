using System;
using System.Collections.Generic;
using Iamport.RestApi.Apis;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아무런 기능 확장없이 빌트인 API만 사용할 수 있는 API Factory.
    /// </summary>
    public sealed class DefaultApiFactory : IApiFactory
    {
        // 빌트인 API
        private static IReadOnlyDictionary<Type, Lazy<IIamportApi>> builtInApis;
        private readonly IIamportClient client;

        /// <summary>
        /// 주어진 아임포트 HTTP 클라이언트로 팩토리를 초기화합니다.
        /// </summary>
        /// <param name="client">아임프토 HTTP 클라이언트</param>
        public DefaultApiFactory(IIamportClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            this.client = client;
            builtInApis = GetBuiltInApis();
        }

        private IReadOnlyDictionary<Type, Lazy<IIamportApi>> GetBuiltInApis()
        {
            return new Dictionary<Type, Lazy<IIamportApi>>
            {
                { typeof(IUsersApi), new Lazy<IIamportApi>(() => new UsersApi(client)) },
                { typeof(IPaymentsApi), new Lazy<IIamportApi>(() => new PaymentsApi(client)) },
                { typeof(ISubscribeApi), new Lazy<IIamportApi>(() => new SubscribeApi(client)) },
            };
        }

        /// <summary>
        /// 주어진 서비스 타입으로 등록된 API 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="apiServiceType">API 서비스 타입</param>
        /// <returns>등록된 API 인스턴스</returns>
        public object GetApi(Type apiServiceType)
        {
            Lazy<IIamportApi> api = null;
            if (builtInApis.TryGetValue(apiServiceType, out api))
            {
                return api.Value;
            }
            return null;
        }
    }
}
