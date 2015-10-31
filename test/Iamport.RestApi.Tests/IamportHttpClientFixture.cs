using Iamport.RestApi.Models;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Iamport.RestApi.Tests
{
    public class IamportHttpClientFixture
    {
        public IamportHttpClient GetDefaultClient()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ImportId"] = "abcd",
                    ["ApiKey"] = "1234",
                    ["ApiSecret"] = "5678",
                })
                .Build();
            var accessor = GetAccessor(configuration);
            return new IamportHttpClient(accessor);
        }
        public IamportHttpClient GetMockClient(TimeSpan? tokenExpiration = null)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ImportId"] = "abcd",
                    ["ApiKey"] = "1234",
                    ["ApiSecret"] = "5678",
                })
                .Build();
            var accessor = GetAccessor(configuration);
            var client = new MockClient(accessor);
            if (tokenExpiration.HasValue == false
                || tokenExpiration.Value == TimeSpan.Zero)
            {
                tokenExpiration = TimeSpan.FromMinutes(10);
            }
            client.TokenExpiration = tokenExpiration.Value;
            return client;
        }

        public IOptions<IamportHttpClientOptions> GetAccessor(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<IamportHttpClientOptions>(configuration);
            services.Configure<IamportHttpClientOptions>(options =>
            {
                options.HttpClientConfigure = client =>
                {
                };
            });
            var provider = services.BuildServiceProvider();
            return provider.GetService<IOptions<IamportHttpClientOptions>>();
        }

        internal class MockClient : IamportHttpClient
        {
            public TimeSpan TokenExpiration { get; set; }
            public IList<HttpRequestMessage> Messages { get; set; } = new List<HttpRequestMessage>();
            public static IList<HttpRequestMessage> GetMessages(IamportHttpClient client)
            {
                if (client is MockClient)
                {
                    return (client as MockClient).Messages;
                }
                return Enumerable.Empty<HttpRequestMessage>().ToList();
            }

            private readonly IamportHttpClientOptions options;
            public MockClient(IOptions<IamportHttpClientOptions> optionsAccessor) : base(optionsAccessor)
            {
                options = optionsAccessor.Value;
            }

            public async override Task<IamportResponse<TResult>> RequestAsync<TResult>(HttpRequestMessage request)
            {
                Messages.Add(request);
                var uri = request.RequestUri;
                if (!uri.IsAbsoluteUri)
                {
                    uri = new Uri(new Uri(options.BaseUrl, UriKind.Absolute), request.RequestUri);
                }
                var path = uri
                    .GetComponents(UriComponents.Path, UriFormat.Unescaped)
                    .Trim('/');
                if (path == "error")
                {
                    return await ParseResponseAsync<TResult>(new HttpResponseMessage
                    {
                        Content = new StringContent(""),
                        StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    });
                }
                else if (path == "users/getToken")
                {
                    object content = new IamportToken
                    {
                        AccessToken = Guid.NewGuid().ToString(),
                        IssuedAt = DateTime.UtcNow,
                        ExpiredAt = DateTime.UtcNow.Add(TokenExpiration)
                    };
                    return new IamportResponse<TResult>
                    {
                        Content = (TResult)content
                    };
                }
                return await ParseResponseAsync<TResult>(new HttpResponseMessage
                {
                    Content = new StringContent(""),
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

        }
    }
}
