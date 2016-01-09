using Iamport.RestApi.Apis;
using System;
using System.Threading.Tasks;
using Xunit;
using Iamport.RestApi.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Iamport.RestApi.Tests.Apis
{
    public class PaymentsApiTest
    {
        [Fact]
        public void GuardClause()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PaymentsApi(null));
        }

        [Fact]
        public async Task CancelAsync_GuardClause()
        {
            var sut = new PaymentsApi(GetClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.CancelAsync(null));
        }
        [Fact]
        public async Task GetAsync_GuardClause()
        {
            var sut = new PaymentsApi(GetClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetAsync(null));
        }
        [Fact]
        public async Task GetByIamportIdAsync_GuardClause()
        {
            var sut = new PaymentsApi(GetClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetByIamportIdAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetByIamportIdAsync(""));
        }
        [Fact]
        public async Task GetByTransactionIdAsync_GuardClause()
        {
            var sut = new PaymentsApi(GetClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetByTransactionIdAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetByTransactionIdAsync(""));
        }
        [Fact]
        public async Task GetPreparationAsync_GuardClause()
        {
            var sut = new PaymentsApi(GetClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetPreparationAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetPreparationAsync(""));
        }
        [Fact]
        public async Task PrepareAsync_GuardClause()
        {
            var sut = new PaymentsApi(GetClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.PrepareAsync(null));
        }

        [Fact]
        public async Task CancelAsync_requests_proper_uri()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedResult = new Payment();
            var sut = new PaymentsApi(client);
            var cancellation = new PaymentCancellation
            {
                IamportId = Guid.NewGuid().ToString()
            };

            // act
            var result = await sut.CancelAsync(cancellation);

            // assert
            Assert.NotNull(result);
            Assert.Equal("payments/cancel", client.RequestedPathAndQuerystrings.Single());
        }

        [Fact]
        public async Task CancelAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedCode = -1;
            var sut = new PaymentsApi(client);
            var cancellation = new PaymentCancellation
            {
                IamportId = Guid.NewGuid().ToString()
            };

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.CancelAsync(cancellation));
        }

        [Theory]
        [InlineData(PaymentQueryState.All, 0, "payments/status/all")]
        [InlineData(PaymentQueryState.All, 1, "payments/status/all?page=1")]
        [InlineData(PaymentQueryState.Paid, 0, "payments/status/paid")]
        [InlineData(PaymentQueryState.Failed, 2, "payments/status/failed?page=2")]
        public async Task GetAsync_requests_proper_uri(PaymentQueryState state, int page, string expectedPath)
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedResult = new PagedResult<Payment>();
            var sut = new PaymentsApi(client);
            var input = new PaymentPageQuery
            {
                Page = page,
                State = state,
            };

            // act
            var result = await sut.GetAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedPath, client.RequestedPathAndQuerystrings.Single());
        }

        [Fact]
        public async Task GetAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedCode = -1;
            var sut = new PaymentsApi(client);

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetAsync(new PaymentPageQuery()));
        }

        [Fact]
        public async Task GetByIamportIdAsync_requests_proper_uri()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedResult = new Payment();
            var sut = new PaymentsApi(client);
            var input = Guid.NewGuid().ToString();

            // act
            var result = await sut.GetByIamportIdAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.Equal($"payments/{input}", client.RequestedPathAndQuerystrings.Single());
        }

        [Fact]
        public async Task GetByIamportIdAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedCode = -1;
            var sut = new PaymentsApi(client);
            var input = Guid.NewGuid().ToString();

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetByIamportIdAsync(input));
        }

        [Fact]
        public async Task GetByTransactionIdAsync_requests_proper_uri()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedResult = new Payment();
            var sut = new PaymentsApi(client);
            var input = Guid.NewGuid().ToString();

            // act
            var result = await sut.GetByTransactionIdAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.Equal($"payments/find/{input}", client.RequestedPathAndQuerystrings.Single());
        }

        [Fact]
        public async Task GetByTransactionIdAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedCode = -1;
            var sut = new PaymentsApi(client);
            var input = Guid.NewGuid().ToString();

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetByTransactionIdAsync(input));
        }

        [Fact]
        public async Task GetPreparationAsync_requests_proper_uri()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedResult = new PaymentPreparation();
            var sut = new PaymentsApi(client);
            var input = Guid.NewGuid().ToString();

            // act
            var result = await sut.GetPreparationAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.Equal($"payments/prepare/{input}", client.RequestedPathAndQuerystrings.Single());
        }

        [Fact]
        public async Task GetPreparationAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedCode = -1;
            var sut = new PaymentsApi(client);
            var input = Guid.NewGuid().ToString();

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetPreparationAsync(input));
        }

        [Fact]
        public async Task PreparationAsync_requests_proper_uri()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedResult = new PaymentPreparation();
            var sut = new PaymentsApi(client);
            var input = new PaymentPreparation
            {
                Amount = 100,
                TransactionId = Guid.NewGuid().ToString(),
            };

            // act
            var result = await sut.PrepareAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.Equal($"payments/prepare", client.RequestedPathAndQuerystrings.Single());
        }

        [Fact]
        public async Task PreparationAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var client = GetClient() as MockClient;
            client.ExpectedCode = -1;
            var sut = new PaymentsApi(client);
            var input = new PaymentPreparation
            {
                Amount = 100,
                TransactionId = Guid.NewGuid().ToString(),
            };

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.PrepareAsync(input));
        }

        protected IIamportHttpClient GetClient()
        {
            var client = new MockClient();
            return client;
        }

        private class MockClient : IIamportHttpClient
        {
            public IamportToken CurrentToken { get; set; }
            public object ExpectedResult { get; set; }
            public int ExpectedCode { get; set; }
            public HttpStatusCode ExpectedHttpStatusCode { get; set; } = HttpStatusCode.OK;
            public IList<string> RequestedPathAndQuerystrings { get; set; } = new List<string>();

            public bool IsAuthorized
            {
                get
                {
                    return CurrentToken != null;
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
                if (CurrentToken == null)
                {
                    throw new HttpRequestException($"Request failed with status code of 400.");
                }
                return await Task.FromResult(CurrentToken);
            }

            public async Task<IamportResponse<TResult>> RequestAsync<TResult>(HttpRequestMessage request)
            {
                return await Task.FromResult(new IamportResponse<TResult>());
            }

            public async Task<IamportResponse<TResult>> RequestAsync<TRequest, TResult>(IamportRequest<TRequest> request)
            {
                RequestedPathAndQuerystrings.Add(request.ApiPathAndQueryString);
                return await Task.FromResult(new IamportResponse<TResult>
                {
                    Code = ExpectedCode,
                    Content = (TResult)ExpectedResult,
                    HttpStatusCode = ExpectedHttpStatusCode,
                });
            }
        }
    }
}
