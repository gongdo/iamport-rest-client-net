using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.DependencyInjection;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트 서버와의 통신을 담당하는 HTTP 클라이언트 클래스.
    /// </summary>
    public class IamportHttpClient : IIamportClient, IDisposable
    {
        private readonly IamportHttpClientOptions options;
        private readonly HttpClient httpClient;
        private long disposalCounter;

        /// <summary>
        /// 응답이 성공임을 나타내는 코드
        /// </summary>
        public const int ResponseSuccessCode = 0;

        /// <summary>
        /// 주어진 파라미터로 Iamport.RestApi.IamportHttpClient의 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="optionsAccessor">아임포트 클라이언트의 옵션 정보 제공자.</param>
        public IamportHttpClient(IOptions<IamportHttpClientOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            options = optionsAccessor.Value;
            ValidateOptions();

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            options.HttpClientConfigure?.Invoke(httpClient);
        }
        
        /// <summary>
        /// 기본 옵션으로 새 아임포트 클라이언트 인스턴스를 반환합니다.
        /// </summary>
        /// <returns>아임포트 클라이언트 인스턴스</returns>
        public static IamportHttpClient Create()
        {
            return Create(options => { });
        }
        /// <summary>
        /// 주어진 옵션으로 새 아임포트 클라이언트 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="options">아임포트 클라이언트 옵션</param>
        /// <returns>아임포트 클라이언트 인스턴스</returns>
        public static IamportHttpClient Create(IamportHttpClientOptions options)
        {
            var accessor = new OptionsManager<IamportHttpClientOptions>(new[] { new ConfigureOptions<IamportHttpClientOptions>(o =>
            {
                o.ApiKey = options.ApiKey;
                o.ApiSecret = options.ApiSecret;
                o.AuthorizationHeaderName = options.AuthorizationHeaderName;
                o.BaseUrl = options.BaseUrl;
                o.HttpClientConfigure = options.HttpClientConfigure;
                o.ImportId = options.ImportId;
            }) });

            return new IamportHttpClient(accessor);
        }
        /// <summary>
        /// 주어진 설정으로 새 아임포트 클라이언트 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="configure">아임포트 클라이언트 설정</param>
        /// <returns>아임포트 클라이언트 인스턴스</returns>
        public static IamportHttpClient Create(Action<IamportHttpClientOptions> configure)
        {
            // 기본 옵션을 설정하는 컨피그는 config.json으로 간주합니다.
            // 이 외의 옵션 지정을 정확히 하려면 클라이언트를 생성하는 코드에서 지정해야 합니다.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json");
            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<IamportHttpClientOptions>(configuration);
            services.Configure(configure);
            var provider = services.BuildServiceProvider();
            var accessor = provider.GetService<IOptions<IamportHttpClientOptions>>();
            return new IamportHttpClient(accessor);
        }

        private void ValidateOptions()
        {
            if (string.IsNullOrEmpty(options.ImportId))
            {
                throw new ArgumentNullException(nameof(options.ImportId));
            }
            if (string.IsNullOrEmpty(options.ApiKey))
            {
                throw new ArgumentNullException(nameof(options.ApiKey));
            }
            if (string.IsNullOrEmpty(options.ApiSecret))
            {
                throw new ArgumentNullException(nameof(options.ApiSecret));
            }
            if (string.IsNullOrEmpty(options.AuthorizationHeaderName))
            {
                throw new ArgumentNullException(nameof(options.AuthorizationHeaderName));
            }
            if (string.IsNullOrEmpty(options.BaseUrl))
            {
                throw new ArgumentNullException(nameof(options.BaseUrl));
            }
        }

        /// <summary>
        /// 현재 인스턴스가 Dispose되었는지 여부를 반환합니다.
        /// </summary>
        public bool IsDisposed { get { return Interlocked.Read(ref disposalCounter) > 0; } }

        private void ThrowsIfDisposed()
        {
            if (IsDisposed)
            {
                throw new InvalidOperationException("The instance is alread disposed.");
            }
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposalCounter, 1, 0) == 0)
            {
                httpClient.Dispose();
            }
        }
    }
}
