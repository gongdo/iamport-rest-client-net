using Iamport.RestApi.Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Iamport.RestApi.Tests
{
    public class IamportHttpClientTest
    {
        private const string DefaultTokenHeaderName = "X-ImpTokenHeader";
        private static readonly IamportResponse<IamportToken> SucceedTokenResponse
            = new IamportResponse<IamportToken>
            {
                Code = 0,
                Content = new IamportToken
                {
                    AccessToken = Guid.NewGuid().ToString(),
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                },
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Message = null,
            };
        private static readonly IamportResponse<object> EmptyObjectResponse
            = new IamportResponse<object>
            {
                Code = 0,
                Content = null,
                Message = null,
            };
        private static readonly IamportResponse<IamportToken> FastExpiredTokenResponse
            = new IamportResponse<IamportToken>
            {
                Code = 0,
                Content = new IamportToken
                {
                    AccessToken = Guid.NewGuid().ToString(),
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddMilliseconds(1),
                },
                Message = null,
            };
        private static readonly IamportResponse<IamportToken> FailedTokenResponse
            = new IamportResponse<IamportToken>
            {
                Code = -1,
                Message = "Failed",
            };

        [Fact]
        public void GuardClause()
        {
            Assert.Throws<ArgumentNullException>(
                () => new IamportHttpClient(null));
        }

        [Fact]
        public void GuardClause_for_options()
        {
            // arrange
            var options = new IamportHttpClientOptions();

            // act/assert
            Assert.Throws<ArgumentNullException>(
                () => new IamportHttpClient(options));
        }

        [Fact]
        public void GuardClause_for_HttpClient()
        {
            // arrange
            var options = GetMockOptions();

            // act/assert
            Assert.Throws<ArgumentNullException>(
                () => new IamportHttpClient(options, null));
        }

        [Fact]
        public void Creates_a_new_instance()
        {
            // arrange/act
            var sut = GetDefaultSut();

            // assert
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData("uuu")]
        [InlineData("a/b/c/d")]
        public void Throws_BaseUrl_is_not_Uri(string value)
        {
            // arrange
            var options = GetMockOptions();
            options.BaseUrl = value;

            // act/assert
            Assert.Throws<UriFormatException>(
                () => new IamportHttpClient(options));
        }

        [Theory]
        [InlineData("files://test.txt")]
        [InlineData("app://test.txt")]
        public void Throws_BaseUrl_is_not_http_nor_https_scheme(string value)
        {
            // arrange
            var options = GetMockOptions();
            options.BaseUrl = value;

            // act/assert
            Assert.Throws<ArgumentException>(
                () => new IamportHttpClient(options));
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
        public async Task Authorize_makes_IsAuthorized_true()
        {
            // arrange
            var options = GetMockOptions();
            var uri = ApiPathUtility.Build(options.BaseUrl, "/users/getToken");
            var mock = new Mock<HttpClient>();
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(uri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new JsonContent(JsonConvert.SerializeObject(SucceedTokenResponse))
            });
            var sut = new IamportHttpClient(options, mock.Object);
            
            // act
            await sut.AuthorizeAsync();

            // assert
            Assert.True(sut.IsAuthorized);
        }

        [Fact]
        public async Task Failed_Authorize_doesnot_change_IsAuthorized()
        {
            // arrange
            var options = GetMockOptions();
            var uri = ApiPathUtility.Build(options.BaseUrl, "/users/getToken");
            var mock = new Mock<HttpClient>();
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(uri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
                Content = new JsonContent(JsonConvert.SerializeObject(FailedTokenResponse))
            });
            var sut = new IamportHttpClient(options, mock.Object);

            // act/assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => sut.AuthorizeAsync());
            Assert.False(sut.IsAuthorized);
        }

        [Fact]
        public async Task IsAuthorized_is_false_after_expiration()
        {
            // arrange
            var options = GetMockOptions();
            var uri = ApiPathUtility.Build(options.BaseUrl, "/users/getToken");
            var mock = new Mock<HttpClient>();
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(uri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new JsonContent(JsonConvert.SerializeObject(FastExpiredTokenResponse))
            });
            var sut = new IamportHttpClient(options, mock.Object);

            // act
            await sut.AuthorizeAsync();
            await Task.Delay(15);
            // assert
            Assert.False(sut.IsAuthorized);
        }
        
        [Fact]
        public async Task Throws_HttpRequestException_for_http_exception()
        {
            // arrange
            var options = GetMockOptions();
            var mock = new Mock<HttpClient>();
            mock.Setup(client => client.SendAsync(
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("exception"));
            var sut = new IamportHttpClient(options, mock.Object);
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/error",
                RequireAuthorization = false
            };
            // act/assert
            await Assert.ThrowsAsync<HttpRequestException>(
                () => sut.RequestAsync<object, object>(request));
        }

        [Fact]
        public async Task RequireAuthorization_ensures_authorization()
        {
            // arrange
            var options = GetMockOptions();
            var uri = ApiPathUtility.Build(options.BaseUrl, "/users/getToken");
            var mock = new Mock<HttpClient>();
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(uri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new JsonContent(JsonConvert.SerializeObject(SucceedTokenResponse))
            });
            var somethingUri = ApiPathUtility.Build(options.BaseUrl, "/something");
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(somethingUri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new JsonContent(JsonConvert.SerializeObject(EmptyObjectResponse))
            });
            var sut = new IamportHttpClient(options, mock.Object);
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/something",
                RequireAuthorization = true
            };
            
            // act
            var result = await sut.RequestAsync<object, object>(request);

            // assert
            mock.Verify(client => client.SendAsync(
                It.Is<HttpRequestMessage>(
                    message => message.RequestUri.Equals(somethingUri)),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RequestIamportRequest_does_not_call_Authorize_before_expired()
        {
            // arrange
            var options = GetMockOptions();
            var uri = ApiPathUtility.Build(options.BaseUrl, "/users/getToken");
            var mock = new Mock<HttpClient>();
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(uri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new JsonContent(JsonConvert.SerializeObject(SucceedTokenResponse))
            });
            var somethingUri = ApiPathUtility.Build(options.BaseUrl, "/something");
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(somethingUri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new JsonContent(JsonConvert.SerializeObject(EmptyObjectResponse))
            });
            var sut = new IamportHttpClient(options, mock.Object);
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/something",
                RequireAuthorization = true
            };

            // act; call twice
            await sut.RequestAsync<object, object>(request);
            await sut.RequestAsync<object, object>(request);

            // assert
            mock.Verify(client => client.SendAsync(
                It.Is<HttpRequestMessage>(
                    message => message.RequestUri.Equals(uri)),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RequestIamportRequest_calls_Authorize_after_expired()
        {
            // arrange
            var options = GetMockOptions();
            var uri = ApiPathUtility.Build(options.BaseUrl, "/users/getToken");
            var mock = new Mock<HttpClient>();
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(uri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new JsonContent(JsonConvert.SerializeObject(FastExpiredTokenResponse))
            });
            var somethingUri = ApiPathUtility.Build(options.BaseUrl, "/something");
            mock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(
                        message => message.RequestUri.Equals(somethingUri)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new JsonContent(JsonConvert.SerializeObject(EmptyObjectResponse))
            });
            var sut = new IamportHttpClient(options, mock.Object);
            var request = new IamportRequest<object>
            {
                ApiPathAndQueryString = "/something",
                RequireAuthorization = true
            };

            // act; call twice
            await sut.RequestAsync<object, object>(request);
            await Task.Delay(15);
            await sut.RequestAsync<object, object>(request);

            // assert
            mock.Verify(client => client.SendAsync(
                It.Is<HttpRequestMessage>(
                    message => message.RequestUri.Equals(uri)),
                    It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        private IamportHttpClientOptions GetMockOptions()
        {
            return new IamportHttpClientOptions
            {
                ApiKey = "asdf",
                ApiSecret = "asdf",
                IamportId = "asdf",
                BaseUrl = "http://fake.fake",
            };
        }

        private IamportHttpClient GetDefaultSut()
        {
            var options = GetMockOptions();
            var mock = new Mock<HttpClient>();
            return new IamportHttpClient(options, mock.Object);
        }

        private class MockHttpClient : HttpClient
        {
            public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
