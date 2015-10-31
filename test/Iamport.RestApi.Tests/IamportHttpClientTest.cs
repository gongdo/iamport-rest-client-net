using Iamport.RestApi.Models;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Iamport.RestApi.Tests
{
    public class IamportHttpClientTest
    {
        private const string DefaultTokenHeaderName = "X-ImpTokenHeader";

        [Fact]
        public void GuardClause()
        {
            Assert.Throws<ArgumentNullException>(
                () => new IamportHttpClient(null));
        }

        [Fact]
        public void Creates_a_new_instance()
        {
            // arrange/act
            var sut = GetDefaultSut();

            // assert
            Assert.NotNull(sut);
        }

        [Fact]
        public void Creates_a_new_instance_via_config_json_file()
        {
            // arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build()
                .GetSection("AppSettings:Iamport:IamportHttpClientOptions");
            var accessor = GetAccessor(configuration);

            // act
            var sut = new IamportHttpClient(accessor);

            // assert
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData("ImportId", null)]
        [InlineData("ApiKey", null)]
        [InlineData("ApiSecret", null)]
        [InlineData("AuthorizationHeaderName", "")]
        [InlineData("BaseUrl", "")]
        public void GuardClause_for_options(string fieldName, string value)
        {
            // arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ImportId"] = "abcd",
                    ["ApiKey"] = "1234",
                    ["ApiSecret"] = "5678",
                    ["AuthorizationHeaderName"] = "xxxx",
                    ["BaseUrl"] = "uuuu",
                })
                .Build();
            configuration[fieldName] = value;
            var accessor = GetAccessor(configuration);

            // act/assert
            Assert.Throws<ArgumentNullException>(
                () => new IamportHttpClient(accessor));
        }

        [Theory]
        [InlineData("uuu")]
        [InlineData("a/b/c/d")]
        public void Throws_BaseUrl_is_not_Uri(string value)
        {
            // arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ImportId"] = "abcd",
                    ["ApiKey"] = "1234",
                    ["ApiSecret"] = "5678",
                    ["BaseUrl"] = value,
                })
                .Build();
            var accessor = GetAccessor(configuration);

            // act/assert
            Assert.Throws<UriFormatException>(
                () => new IamportHttpClient(accessor));
        }

        [Theory]
        [InlineData("files://test.txt")]
        [InlineData("app://test.txt")]
        public void Throws_BaseUrl_is_not_http_nor_https_scheme(string value)
        {
            // arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ImportId"] = "abcd",
                    ["ApiKey"] = "1234",
                    ["ApiSecret"] = "5678",
                    ["BaseUrl"] = value,
                })
                .Build();
            var accessor = GetAccessor(configuration);

            // act/assert
            Assert.Throws<ArgumentException>(
                () => new IamportHttpClient(accessor));
        }

        [Fact]
        public void Disposes()
        {
            // arrange
            var sut = GetDefaultSut();
            // act
            sut.Dispose();
            // assert
            Assert.True(sut.IsDisposed);
        }

        [Fact]
        public async Task GuardClause_RequestIamportRequest()
        {
            // arrange
            var sut = GetDefaultSut();
            // act/assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.RequestAsync<object, object>(null));
        }

        [Fact]
        public async Task GuardClause_RequestHttpRequest()
        {
            // arrange
            var sut = GetDefaultSut();
            // act/assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.RequestAsync<object>(null));
        }

        [Fact]
        public async Task RequestIamportRequest_throws_if_disposed()
        {
            // arrange
            var sut = GetDefaultSut();
            sut.Dispose();
            // act/assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.RequestAsync<object, object>(new IamportRequest<object>()));
        }

        [Fact]
        public async Task RequestHttpRequest_throws_if_disposed()
        {
            // arrange
            var sut = GetDefaultSut();
            sut.Dispose();
            // act/assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.RequestAsync<object>(new HttpRequestMessage()));
        }

        [Fact]
        public async Task Authorize_throws_if_disposed()
        {
            // arrange
            var sut = GetDefaultSut();
            sut.Dispose();
            // act/assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.AuthorizeAsync());
        }

        [Fact]
        public async Task Authorize_calls_RequestHttpRequest()
        {
            // arrange
            var sut = GetMockSut();
            var defaultOptions = new IamportHttpClientOptions();
            var expectedUrl = ApiPathUtility.Build(defaultOptions.BaseUrl, "/users/getToken");
            // act
            await sut.AuthorizeAsync();
            // assert
            Assert.Equal(expectedUrl, MockClient.GetMessages(sut).Single().RequestUri.ToString());
        }

        [Fact]
        public async Task RequestIamportRequest_calls_self_url()
        {
            // arrange
            var sut = GetMockSut();
            var defaultOptions = new IamportHttpClientOptions();
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/self",
                RequireAuthorization = false
            };
            var expectedUrl = ApiPathUtility.Build(defaultOptions.BaseUrl, "/self");
            // act
            var result = await sut.RequestAsync<object, object>(request);
            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedUrl, MockClient.GetMessages(sut).Single().RequestUri.ToString());
        }

        // TODO: HttpClient가 토큰 헤더를 붙여서 호출하는지 테스트할 수 없음.
        // HttpClient 구성을 변경하는 것이 좋겠음.
        //[Fact]
        public async Task RequestIamportRequest_has_token_header()
        {
            // arrange
            var sut = GetMockSut();
            var defaultOptions = new IamportHttpClientOptions();
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/self",
                RequireAuthorization = true
            };
            // act
            var result = await sut.RequestAsync<object, object>(request);
            // assert
            var actual = MockClient.GetMessages(sut)
                .Last()
                .Headers
                .GetValues(DefaultTokenHeaderName)
                .Any();
            Assert.True(actual);
        }

        [Fact]
        public async Task RequestIamportRequest_throws_HttpRequestException_for_http_exception()
        {
            // arrange
            var sut = GetMockSut();
            var defaultOptions = new IamportHttpClientOptions();
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/error",
                RequireAuthorization = false
            };
            // act/assert
            await Assert.ThrowsAsync<HttpRequestException>(() => sut.RequestAsync<object, object>(request));
        }

        [Fact]
        public async Task RequestIamportRequest_calls_also_Authorize_and_self()
        {
            // arrange
            var sut = GetMockSut();
            var defaultOptions = new IamportHttpClientOptions();
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/self",
                RequireAuthorization = true
            };
            var expectedAuthUrl = ApiPathUtility.Build(defaultOptions.BaseUrl, "/users/getToken");
            var expectedUrl = ApiPathUtility.Build(defaultOptions.BaseUrl, "/self");
            // act
            var result = await sut.RequestAsync<object, object>(request);
            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedAuthUrl, MockClient.GetMessages(sut).First().RequestUri.ToString());
            Assert.Equal(expectedUrl, MockClient.GetMessages(sut).Last().RequestUri.ToString());
        }

        [Fact]
        public async Task RequestIamportRequest_does_not_call_Authorize_before_expired()
        {
            // arrange
            var sut = GetMockSut();
            var defaultOptions = new IamportHttpClientOptions();
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/self",
                RequireAuthorization = true
            };
            var expectedAuthUrl = ApiPathUtility.Build(defaultOptions.BaseUrl, "/users/getToken");
            // act
            // call twice
            await sut.RequestAsync<object, object>(request);
            var result = await sut.RequestAsync<object, object>(request);
            // assert
            Assert.NotNull(result);
            var actual = MockClient.GetMessages(sut)
                .Count(m => m.RequestUri.ToString() == expectedAuthUrl);
            Assert.Equal(1, actual);
        }

        [Fact]
        public async Task RequestIamportRequest_calls_Authorize_after_expired()
        {
            // arrange
            var sut = GetMockSut(TimeSpan.FromMilliseconds(1));
            var defaultOptions = new IamportHttpClientOptions();
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/self",
                RequireAuthorization = true
            };
            var expectedAuthUrl = ApiPathUtility.Build(defaultOptions.BaseUrl, "/users/getToken");
            // act
            // call twice
            await sut.RequestAsync<object, object>(request);
            var result = await sut.RequestAsync<object, object>(request);
            // assert
            Assert.NotNull(result);
            var actual = MockClient.GetMessages(sut)
                .Count(m => m.RequestUri.ToString() == expectedAuthUrl);
            Assert.Equal(2, actual);
        }

        private IamportHttpClient GetDefaultSut()
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
        private IamportHttpClient GetMockSut(TimeSpan? tokenExpiration = null)
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
        private IOptions<IamportHttpClientOptions> GetAccessor(IConfiguration configuration)
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

        private class MockClient : IamportHttpClient
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
                var path = request.RequestUri
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
