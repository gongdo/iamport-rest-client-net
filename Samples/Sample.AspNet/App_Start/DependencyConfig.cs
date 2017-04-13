using Autofac;
using Autofac.Integration.Mvc;
using Iamport.RestApi;
using Iamport.RestApi.Apis;
using Sample.AspNetCore.Repositories;
using System.Configuration;
using System.Web.Mvc;

namespace Sample.AspNet
{
    public class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Register services.
            builder
                .RegisterType<PaymentRepository>()
                .SingleInstance();
            builder
                .RegisterType<CheckoutRepository>()
                .SingleInstance();

            var clientOptions = GetHttpClientOptions();
            builder.RegisterInstance(clientOptions);

            builder
                .RegisterType<IamportHttpClient>()
                .As<IIamportClient>()
                .SingleInstance();

            builder
                .RegisterType<PaymentsApi>()
                .As<IPaymentsApi>()
                .SingleInstance();

            builder
                .RegisterType<SubscribeApi>()
                .As<ISubscribeApi>()
                .SingleInstance();

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(DependencyConfig).Assembly);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        /// <summary>
        /// ConfigurationManager에서 아임포트 설정을 반환합니다.
        /// </summary>
        /// <returns>아임포트 설정</returns>
        private static IamportHttpClientOptions GetHttpClientOptions()
        {
            // 다음은 설정의 예입니다.
            // 반드시 web.config 설정에 API 항목을 설정해야 합니다.
            var apiKey = ConfigurationManager.AppSettings["iamport:ApiKey"];
            var apiSecret = ConfigurationManager.AppSettings["iamport:ApiSecret"];
            var iamportId = ConfigurationManager.AppSettings["iamport:IamportId"];
            var baseUrl = ConfigurationManager.AppSettings["iamport:BaseUrl"];
            var authorizationHeaderName = ConfigurationManager.AppSettings["iamport:AuthorizationHeaderName"];
            var options = new IamportHttpClientOptions();
            if (!string.IsNullOrEmpty(apiKey))
            {
                options.ApiKey = apiKey;
            }
            if (!string.IsNullOrEmpty(apiSecret))
            {
                options.ApiSecret = apiSecret;
            }
            if (!string.IsNullOrEmpty(iamportId))
            {
                options.IamportId = iamportId;
            }
            if (!string.IsNullOrEmpty(baseUrl))
            {
                options.BaseUrl = baseUrl;
            }
            if (!string.IsNullOrEmpty(authorizationHeaderName))
            {
                options.AuthorizationHeaderName = authorizationHeaderName;
            }
            return options;
        }
    }
}