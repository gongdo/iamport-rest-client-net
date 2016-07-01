using Iamport.RestApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Iamport.RestApi.Tests
{
    public class IamportHttpClientTest : IClassFixture<IamportHttpClientFixture>
    {
        private const string DefaultTokenHeaderName = "X-ImpTokenHeader";

        private readonly IamportHttpClientFixture fixture;
        public IamportHttpClientTest(IamportHttpClientFixture fixture)
        {
            this.fixture = fixture;
        }

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
            Assert.Equal(expectedUrl, IamportHttpClientFixture.MockClient.GetMessages(sut).Single().RequestUri.ToString());
        }

        [Fact]
        public async Task Authorize_makes_IsAuthorized_true()
        {
            // arrange
            var sut = GetMockSut();
            // act
            await sut.AuthorizeAsync();
            // assert
            Assert.True(sut.IsAuthorized);
        }

        [Fact]
        public async Task IsAuthorized_is_false_after_expiration()
        {
            // arrange
            var sut = GetMockSut(TimeSpan.FromMilliseconds(15));
            // act
            await sut.AuthorizeAsync();
            await Task.Delay(15);
            // assert
            Assert.False(sut.IsAuthorized);
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
            Assert.Equal(expectedUrl, IamportHttpClientFixture.MockClient.GetMessages(sut).Single().RequestUri.ToString());
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
            var actual = IamportHttpClientFixture.MockClient.GetMessages(sut)
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
            Assert.Equal(expectedAuthUrl, IamportHttpClientFixture.MockClient.GetMessages(sut).First().RequestUri.ToString());
            Assert.Equal(expectedUrl, IamportHttpClientFixture.MockClient.GetMessages(sut).Last().RequestUri.ToString());
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
            var actual = IamportHttpClientFixture.MockClient.GetMessages(sut)
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
            var actual = IamportHttpClientFixture.MockClient.GetMessages(sut)
                .Count(m => m.RequestUri.ToString() == expectedAuthUrl);
            Assert.Equal(2, actual);
        }

        private IamportHttpClient GetDefaultSut()
        {
            return fixture.GetDefaultClient();
        }
        private IamportHttpClient GetMockSut(TimeSpan? tokenExpiration = null)
        {
            return fixture.GetMockClient(tokenExpiration);
        }
        private IOptions<IamportHttpClientOptions> GetAccessor(IConfiguration configuration)
        {
            return fixture.GetAccessor(configuration);
        }
    }
}
