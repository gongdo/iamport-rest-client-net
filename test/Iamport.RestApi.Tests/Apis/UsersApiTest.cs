using Iamport.RestApi.Apis;
using System;
using System.Threading.Tasks;
using Xunit;
using Iamport.RestApi.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace Iamport.RestApi.Tests.Apis
{
    public class UsersApiTest
    {
        [Fact]
        public void GuardClause()
        {
            Assert.Throws<ArgumentNullException>(
                () => new UsersApi(null));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData(null, "secret")]
        [InlineData("key", null)]
        public async Task GetTokenAsync_throws_JsonSerializationException_with_empty_or_null(string key, string secret)
        {
            // arrange
            var client = GetMockClient();
            var sut = new UsersApi(client);
            var request = new IamportTokenRequest
            {
                ApiKey = key,
                ApiSecret = secret
            };

            // act/assert
            await Assert.ThrowsAsync<JsonSerializationException>(
                () => sut.GetTokenAsync(request));
        }

        [Fact]
        public async Task GetTokenAsync_throws_IamportResponseException_with_invalid_key_and_secret()
        {
            // arrange
            var client = GetMockClient();
            var sut = new UsersApi(client);
            var request = new IamportTokenRequest
            {
                ApiKey = "invalid",
                ApiSecret = "invalid"
            };

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetTokenAsync(request));
        }

        [Fact]
        public async Task GetTokenAsync_returns_token()
        {
            // arrange
            var client = GetMockClient();
            var sut = new UsersApi(client);
            var request = new IamportTokenRequest
            {
                ApiKey = "key",
                ApiSecret = "secret"
            };

            // act
            var result = await sut.GetTokenAsync(request);

            // assert
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.True(result.ExpiredAt >= DateTime.UtcNow);
        }

        private IIamportHttpClient GetMockClient()
        {
            var client = new MockClient();
            client.ExpectedTokenRequest = new IamportTokenRequest
            {
                ApiKey = "key",
                ApiSecret = "secret"
            };
            return client;
        }

        private class MockClient : IIamportHttpClient
        {
            public IamportTokenRequest ExpectedTokenRequest { get; set; }

            public bool IsAuthorized
            {
                get
                {
                    return true;
                }
            }

            private IamportToken GetValidToken()
            {
                return new IamportToken
                {
                    AccessToken = Guid.NewGuid().ToString(),
                    ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                    IssuedAt = DateTime.UtcNow,
                };
            }

            public async Task<IamportToken> AuthorizeAsync()
            {
                return await Task.FromResult(
                    GetValidToken());
            }

            public async Task<IamportResponse<TResult>> RequestAsync<TResult>(HttpRequestMessage request)
            {
                var path = request.RequestUri.ToString().Trim('/');
                if (request.RequestUri.IsAbsoluteUri)
                {
                    path = request.RequestUri
                    .GetComponents(UriComponents.Path, UriFormat.Unescaped)
                    .Trim('/');
                }
                if (path.Equals("users/getToken", StringComparison.OrdinalIgnoreCase))
                {
                    var actualRequest = JsonConvert
                        .DeserializeObject<IamportTokenRequest>(
                        await request.Content.ReadAsStringAsync());
                    // simulate validation
                    if (actualRequest.ApiKey != ExpectedTokenRequest.ApiKey
                        || actualRequest.ApiSecret != ExpectedTokenRequest.ApiSecret)
                    {
                        return new IamportResponse<TResult>
                        {
                            Code = -1,
                            Message = "인증에 실패하였습니다. API키와 secret을 확인하세요.",
                        };
                    }

                    object result = GetValidToken();
                    return await Task.FromResult(
                        new IamportResponse<TResult>
                        {
                            Content = (TResult)result
                        });
                }

                return await Task.FromResult(
                    new IamportResponse<TResult>());
            }

            public async Task<IamportResponse<TResult>> RequestAsync<TRequest, TResult>(IamportRequest<TRequest> request)
            {
                if (typeof(TRequest).Equals(typeof(IamportTokenRequest))
                    && ExpectedTokenRequest != null)
                {
                    var actualRequest = request.Content as IamportTokenRequest;
                    // simulate validation
                    if (actualRequest.ApiKey != ExpectedTokenRequest.ApiKey
                        || actualRequest.ApiSecret != ExpectedTokenRequest.ApiSecret)
                    {
                        return await Task.FromResult(new IamportResponse<TResult>
                        {
                            Code = -1,
                            Message = "인증에 실패하였습니다. API키와 secret을 확인하세요.",
                        });
                    }

                    object result = GetValidToken();
                    return await Task.FromResult(
                        new IamportResponse<TResult>
                        {
                            Content = (TResult)result
                        });
                }

                return await Task.FromResult(
                    new IamportResponse<TResult>());
            }
        }
    }
}
