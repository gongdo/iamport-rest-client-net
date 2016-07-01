using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아무런 기능 확장없이 빌트인 API만 사용할 수 있는 API Factory.
    /// </summary>
    public sealed class DefaultApiFactory : ServiceProviderApiFactory
    {
        public DefaultApiFactory(IIamportHttpClient client) : base(GetDefaultServiceProvider(client))
        {
        }

        private static IServiceProvider GetDefaultServiceProvider(IIamportHttpClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var types = typeof(DefaultApiFactory).GetTypeInfo().Assembly.ExportedTypes;
            var apiType = typeof(IIamportApi);

            var apiTypes = types.Where(t =>
                t.GetTypeInfo().IsPublic
                && t.GetInterfaces().Contains(apiType)
            ).ToList();
            var apiInterfaces = apiTypes.Where(t => t.GetTypeInfo().IsInterface).ToList();
            var apiTypeMap = apiInterfaces.Select(i =>
            {
                var matchedImplementation = apiTypes.Where(t =>
                    t.GetInterfaces().Contains(i)
                    && t.GetTypeInfo().IsAbstract == false
                ).FirstOrDefault();
                return new { Interface = i, Implementation = matchedImplementation };
            }).Where(m => m.Implementation != null)
            .ToDictionary(m => m.Interface, m => m.Implementation);

            var services = new ServiceCollection();
            services.AddSingleton(client);
            foreach (var map in apiTypeMap)
            {
                services.AddSingleton(map.Key, map.Value);
            }

            return services.BuildServiceProvider();
        }
    }
}
