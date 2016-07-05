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
using Moq;
using System.ComponentModel.DataAnnotations;

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
            var sut = new PaymentsApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.CancelAsync(null));
        }
        [Fact]
        public async Task GetAsync_GuardClause()
        {
            var sut = new PaymentsApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetAsync(null));
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetByIamportIdAsync_GuardClause(string id)
        {
            var sut = new PaymentsApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetByIamportIdAsync(id));
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetByTransactionIdAsync_GuardClause(string id)
        {
            var sut = new PaymentsApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetByTransactionIdAsync(id));
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetPreparationAsync_GuardClause(string id)
        {
            var sut = new PaymentsApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetPreparationAsync(id));
        }
        [Fact]
        public async Task PrepareAsync_GuardClause()
        {
            var sut = new PaymentsApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.PrepareAsync(null));
        }

        [Fact]
        public async Task CancelAsync_requests_proper_uri()
        {
            // arrange
            var expectedRequest = new PaymentCancellation
            {
                IamportId = Guid.NewGuid().ToString(),
            };
            var expectedResult = new IamportResponse<Payment>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new Payment
                {
                    IamportId = expectedRequest.IamportId,
                }
            };
            var client = GetMockClient(expectedRequest, expectedResult);
            var sut = new PaymentsApi(client);

            // act
            var result = await sut.CancelAsync(expectedRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<PaymentCancellation, Payment>(
                        It.Is<IamportRequest<PaymentCancellation>>(req =>
                            req.ApiPathAndQueryString.EndsWith("payments/cancel"))));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        public async Task CancelAsync_throws_ValidationException(string iamportId, string transactionId)
        {
            // arrange
            var expectedRequest = new PaymentCancellation
            {
                IamportId = iamportId,
                TransactionId = transactionId,
            };
            var expectedResult = new IamportResponse<Payment>();
            var client = GetMockClient(expectedRequest, expectedResult);
            var sut = new PaymentsApi(client);

            // act/assert
            await Assert.ThrowsAsync<ValidationException>(
                () => sut.CancelAsync(expectedRequest));
        }

        [Fact]
        public async Task CancelAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var expectedRequest = new PaymentCancellation
            {
                IamportId = Guid.NewGuid().ToString(),
            };
            var expectedResult = new IamportResponse<Payment>
            {
                Code = -1,
                HttpStatusCode = HttpStatusCode.InternalServerError,
            };
            var client = GetMockClient(expectedRequest, expectedResult);
            var sut = new PaymentsApi(client);
            
            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.CancelAsync(expectedRequest));
        }

        [Theory]
        [InlineData(PaymentQueryState.All, 0, "payments/status/all")]
        [InlineData(PaymentQueryState.All, 1, "payments/status/all?page=1")]
        [InlineData(PaymentQueryState.Paid, 0, "payments/status/paid")]
        [InlineData(PaymentQueryState.Failed, 2, "payments/status/failed?page=2")]
        public async Task GetAsync_requests_proper_uri(PaymentQueryState state, int page, string expectedPath)
        {
            // arrange
            var expectedRequest = new PaymentPageQuery
            {
                State = state,
                Page = page,
            };
            var expectedResult = new IamportResponse<PagedResult<Payment>>
            {
                HttpStatusCode = HttpStatusCode.OK,
            };
            var client = GetMockClient(expectedResult);
            var sut = new PaymentsApi(client);

            // act
            var result = await sut.GetAsync(expectedRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<object, PagedResult<Payment>>(
                        It.Is<IamportRequest>(req =>
                            req.ApiPathAndQueryString.EndsWith(expectedPath))));
        }

        [Fact]
        public async Task GetAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var expectedRequest = new PaymentPageQuery
            {
            };
            var expectedResult = new IamportResponse<PagedResult<Payment>>
            {
                Code = -1,
                HttpStatusCode = HttpStatusCode.InternalServerError,
            };
            var client = GetMockClient(expectedResult);
            var sut = new PaymentsApi(client);

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetAsync(expectedRequest));
        }

        [Fact]
        public async Task GetByIamportIdAsync_requests_proper_uri()
        {
            // arrange
            var expectedRequest = Guid.NewGuid().ToString();
            var expectedResult = new IamportResponse<Payment>
            {
                HttpStatusCode = HttpStatusCode.OK,
            };
            var expectedPath = $"payments/{expectedRequest}";
            var client = GetMockClient(expectedResult);
            var sut = new PaymentsApi(client);

            // act
            var result = await sut.GetByIamportIdAsync(expectedRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<object, Payment>(
                        It.Is<IamportRequest>(req =>
                            req.ApiPathAndQueryString.EndsWith(expectedPath))));
        }

        [Fact]
        public async Task GetByIamportIdAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var expectedRequest = Guid.NewGuid().ToString();
            var expectedResult = new IamportResponse<Payment>
            {
                Code = -1,
                HttpStatusCode = HttpStatusCode.InternalServerError,
            };
            var client = GetMockClient(expectedResult);
            var sut = new PaymentsApi(client);

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetByIamportIdAsync(expectedRequest));
        }

        [Fact]
        public async Task GetByTransactionIdAsync_requests_proper_uri()
        {
            // arrange
            var expectedRequest = Guid.NewGuid().ToString();
            var expectedResult = new IamportResponse<Payment>
            {
                HttpStatusCode = HttpStatusCode.OK,
            };
            var expectedPath = $"payments/find/{expectedRequest}";
            var client = GetMockClient(expectedResult);
            var sut = new PaymentsApi(client);

            // act
            var result = await sut.GetByTransactionIdAsync(expectedRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<object, Payment>(
                        It.Is<IamportRequest>(req =>
                            req.ApiPathAndQueryString.EndsWith(expectedPath))));
        }

        [Fact]
        public async Task GetByTransactionIdAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var expectedRequest = Guid.NewGuid().ToString();
            var expectedResult = new IamportResponse<Payment>
            {
                Code = -1,
                HttpStatusCode = HttpStatusCode.InternalServerError,
            };
            var client = GetMockClient(expectedResult);
            var sut = new PaymentsApi(client);

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetByTransactionIdAsync(expectedRequest));
        }

        [Fact]
        public async Task GetPreparationAsync_requests_proper_uri()
        {
            // arrange
            var expectedRequest = Guid.NewGuid().ToString();
            var expectedResult = new IamportResponse<PaymentPreparation>
            {
                HttpStatusCode = HttpStatusCode.OK,
            };
            var expectedPath = $"payments/prepare/{expectedRequest}";
            var client = GetMockClient(expectedResult);
            var sut = new PaymentsApi(client);

            // act
            var result = await sut.GetPreparationAsync(expectedRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<object, PaymentPreparation>(
                        It.Is<IamportRequest>(req =>
                            req.ApiPathAndQueryString.EndsWith(expectedPath))));
        }

        [Fact]
        public async Task GetPreparationAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var expectedRequest = Guid.NewGuid().ToString();
            var expectedResult = new IamportResponse<PaymentPreparation>
            {
                Code = -1,
                HttpStatusCode = HttpStatusCode.InternalServerError,
            };
            var client = GetMockClient(expectedResult);
            var sut = new PaymentsApi(client);

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.GetPreparationAsync(expectedRequest));
        }

        [Fact]
        public async Task PreparationAsync_requests_proper_uri()
        {
            // arrange
            var expectedRequest = new PaymentPreparation
            {
                Amount = 1000,
                TransactionId = Guid.NewGuid().ToString(),
            };
            var expectedResult = new IamportResponse<PaymentPreparation>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new PaymentPreparation
                {
                    TransactionId = expectedRequest.TransactionId,
                    Amount = expectedRequest.Amount,
                }
            };
            var expectedPath = $"payments/prepare";
            var client = GetMockClient(expectedRequest, expectedResult);
            var sut = new PaymentsApi(client);

            // act
            var result = await sut.PrepareAsync(expectedRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<PaymentPreparation, PaymentPreparation>(
                        It.Is<IamportRequest<PaymentPreparation>>(req =>
                            req.ApiPathAndQueryString.EndsWith(expectedPath))));
        }

        [Theory]
        [InlineData(999, null)]
        [InlineData(1000, null)]
        [InlineData(1000, "123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
        public async Task PreparationAsync_throws_ValidationException(int amount, string transactionId)
        {
            // arrange
            var expectedRequest = new PaymentPreparation
            {
                Amount = amount,
                TransactionId = transactionId,
            };
            var expectedResult = new IamportResponse<PaymentPreparation>();
            var client = GetMockClient(expectedRequest, expectedResult);
            var sut = new PaymentsApi(client);

            // act/assert
            await Assert.ThrowsAsync<ValidationException>(
                () => sut.PrepareAsync(expectedRequest));
        }

        [Fact]
        public async Task PreparationAsync_throws_IamportResponseException_when_response_code_is_not_success()
        {
            // arrange
            var expectedRequest = new PaymentPreparation
            {
                Amount = 1000,
                TransactionId = Guid.NewGuid().ToString(),
            };
            var expectedResult = new IamportResponse<PaymentPreparation>
            {
                Code = -1,
                HttpStatusCode = HttpStatusCode.InternalServerError,
            };
            var client = GetMockClient(expectedRequest, expectedResult);
            var sut = new PaymentsApi(client);

            // act/assert
            await Assert.ThrowsAsync<IamportResponseException>(
                () => sut.PrepareAsync(expectedRequest));
        }

        private IIamportClient GetEmptyMockClient()
        {
            var mock = new Mock<IIamportClient>();
            return mock.Object;
        }

        private IIamportClient GetMockClient<TRequestContent, TResult>(
            TRequestContent expectedContent,
            IamportResponse<TResult> expectedResponse)
        {
            var mock = new Mock<IIamportClient>();
            mock.Setup(client =>
                client.RequestAsync<TRequestContent, TResult>(
                    It.Is<IamportRequest<TRequestContent>>(
                        request => request.Content.Equals(expectedContent))))
            .ReturnsAsync(expectedResponse);
            return mock.Object;
        }

        private IIamportClient GetMockClient<TResult>(
            IamportResponse<TResult> expectedResponse)
        {
            var mock = new Mock<IIamportClient>();
            mock.Setup(client =>
                client.RequestAsync<object, TResult>(
                    It.IsNotNull<IamportRequest>()))
            .ReturnsAsync(expectedResponse);
            return mock.Object;
        }
    }
}
