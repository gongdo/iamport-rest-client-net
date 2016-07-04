using Iamport.RestApi.Apis;
using System;
using System.Threading.Tasks;
using Xunit;
using Iamport.RestApi.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Moq;
using System.Threading;

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
        public async Task GetTokenAsync_throws_UnauthorizedAccessException_with_empty_or_null(string key, string secret)
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
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => sut.GetTokenAsync(request));
        }

        [Fact]
        public async Task GetTokenAsync_throws_UnauthorizedAccessException_with_invalid_key_and_secret()
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
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
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

        private IIamportClient GetMockClient()
        {
            var mock = new Mock<IIamportClient>();
            mock.Setup(client =>
                client.RequestAsync<IamportTokenRequest, IamportToken>(
                    It.Is<IamportRequest<IamportTokenRequest>>(
                        request => request.Content.ApiKey == "key"
                        && request.Content.ApiSecret == "secret")))
            .ReturnsAsync(new IamportResponse<IamportToken>
            {
                Code = 0,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Content = new IamportToken
                {
                    AccessToken = Guid.NewGuid().ToString(),
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                },
                Message = null,
            });
            mock.Setup(client =>
                client.RequestAsync<IamportTokenRequest, IamportToken>(
                    It.Is<IamportRequest<IamportTokenRequest>>(
                        request => request.Content.ApiKey != "key"
                        || request.Content.ApiSecret != "secret")))
            .Throws<UnauthorizedAccessException>();

            return mock.Object;
        }
    }
}
