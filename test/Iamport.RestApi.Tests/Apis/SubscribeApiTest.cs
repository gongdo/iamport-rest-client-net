using Iamport.RestApi.Apis;
using System;
using System.Threading.Tasks;
using Xunit;
using Iamport.RestApi.Models;
using System.Net;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace Iamport.RestApi.Tests.Apis
{
    public class SubscribeApiTest
    {
        [Fact]
        public void GuardClause()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SubscribeApi(null));
        }

        [Fact]
        public async Task RegisterCustomerAsync_GuardClause()
        {
            var sut = new SubscribeApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.RegisterCustomerAsync(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetCustomerAsync_GuardClause(string id)
        {
            var sut = new SubscribeApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.GetCustomerAsync(id));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task DeleteCustomerAsync_GuardClause(string id)
        {
            var sut = new SubscribeApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.DeleteCustomerAsync(id));
        }

        [Fact]
        public async Task DoDirectPaymentAsync_onetime_GuardClause()
        {
            var sut = new SubscribeApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.DoDirectPaymentAsync((DirectPaymentRequest)null));
        }

        [Fact]
        public async Task DoDirectPaymentAsync_again_GuardClause()
        {
            var sut = new SubscribeApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.DoDirectPaymentAsync((CustomerDirectPaymentRequest)null));
        }

        [Fact]
        public async Task SchedulePaymentsAsync_GuardClause()
        {
            var sut = new SubscribeApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.SchedulePaymentsAsync(null));
        }

        [Fact]
        public async Task UnschedulePaymentsAsync_GuardClause()
        {
            var sut = new SubscribeApi(GetEmptyMockClient());
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.UnschedulePaymentsAsync(null));
        }

        [Fact]
        public async Task RegisterCustomerAsync_requests_proper_api()
        {
            // arrange
            var aRequest = new CustomerRegistration
            {
                AuthenticationNumber = "123456",
                CardNumber = "1234-1234-1234-1234",
                Expiry = "2200-12",
                Id = Guid.NewGuid().ToString(),
                PartialPassword = "12",
            };
            var expectedResult = new IamportResponse<Customer>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new Customer
                {
                    Id = aRequest.Id,
                    InsertedTime = DateTime.UtcNow,
                }
            };
            var client = GetMockClient(aRequest, expectedResult);
            var sut = new SubscribeApi(client);

            // act
            var result = await sut.RegisterCustomerAsync(aRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<CustomerRegistration, Customer>(
                        It.Is<IamportRequest<CustomerRegistration>>(req =>
                            req.Method == HttpMethod.Post
                            && req.Content == aRequest
                            && req.ApiPathAndQueryString.EndsWith($"subscribe/customers/{aRequest.Id}"))));
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("idididi", "invalid-card-number", "2100-12", "900101", "12")]
        [InlineData("idididi", "1234-1234-1234-1234", "invalid-expiry", "900101", "12")]
        [InlineData("idididi", "1234-1234-1234-1234", "2100-12", "invalid-number", "12")]
        [InlineData("idididi", "1234-1234-1234-1234", "2100-12", "900101", "invalid")]
        public async Task RegisterCustomerAsync_throws_ValidationException(
            string id,
            string cardNumber,
            string expiry,
            string authentication,
            string partialPassword)
        {
            // arrange
            var aRequest = new CustomerRegistration
            {
                Id = id,
                CardNumber = cardNumber,
                Expiry = expiry,
                AuthenticationNumber = authentication,
                PartialPassword = partialPassword,
            };
            var client = Mock.Of<IIamportClient>();
            var sut = new SubscribeApi(client);

            // act/assert
            await Assert.ThrowsAsync<ValidationException>(
                () => sut.RegisterCustomerAsync(aRequest));
        }

        [Theory]
        [InlineData("customer")]
        [InlineData("홍길동")]
        [InlineData("abcd:홍길동")]
        [InlineData("abcd/홍길동")]
        [InlineData("abcd?홍길동")]
        public async Task GetCustomerAsync_requests_proper_api(string customerId)
        {
            // arrange
            var expectedResult = new IamportResponse<Customer>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new Customer
                {
                    Id = customerId,
                    InsertedTime = DateTime.UtcNow,
                }
            };
            var client = GetMockClient(expectedResult);
            var sut = new SubscribeApi(client);

            // act
            var result = await sut.GetCustomerAsync(customerId);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<object, Customer>(
                        It.Is<IamportRequest>(req =>
                            req.Method == HttpMethod.Get
                            && req.Content == null
                            && req.ApiPathAndQueryString.EndsWith($"subscribe/customers/{WebUtility.UrlEncode(customerId)}"))));
        }

        [Fact]
        public async Task DeleteCustomerAsync_requests_proper_api()
        {
            // arrange
            var customerId = Guid.NewGuid().ToString();
            var expectedResult = new IamportResponse<Customer>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new Customer
                {
                    Id = customerId,
                    InsertedTime = DateTime.UtcNow,
                }
            };
            var client = GetMockClient(expectedResult);
            var sut = new SubscribeApi(client);

            // act
            var result = await sut.DeleteCustomerAsync(customerId);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<object, Customer>(
                        It.Is<IamportRequest>(req =>
                            req.Method == HttpMethod.Delete
                            && req.Content == null
                            && req.ApiPathAndQueryString.EndsWith($"subscribe/customers/{customerId}"))));
        }

        [Fact]
        public async Task DoDirectPaymentAsync_requests_proper_api()
        {
            // arrange
            var aRequest = new DirectPaymentRequest
            {
                Amount = 1024,
                TransactionId = Guid.NewGuid().ToString(),
                AuthenticationNumber = "123456",
                CardNumber = "1234-1234-1234-1234",
                Expiry = "2200-12",
                PartialPassword = "12",
            };
            var expectedResult = new IamportResponse<Payment>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new Payment
                {
                    TransactionId = aRequest.TransactionId,
                    Amount = aRequest.Amount,
                }
            };
            var client = GetMockClient(aRequest, expectedResult);
            var sut = new SubscribeApi(client);

            // act
            var result = await sut.DoDirectPaymentAsync(aRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<DirectPaymentRequest, Payment>(
                        It.Is<IamportRequest<DirectPaymentRequest>>(req =>
                            req.Method == HttpMethod.Post
                            && req.Content == aRequest
                            && req.ApiPathAndQueryString.EndsWith($"subscribe/payments/onetime"))));
        }

        [Theory]
        [InlineData(0, null, null, null, null, null)]
        [InlineData(0, "idididi", "1234-1234-1234-1234", "2100-12", "900101", "12")]
        [InlineData(1024, "idididi", "invalid-card-number", "2100-12", "900101", "12")]
        [InlineData(1024, "idididi", "1234-1234-1234-1234", "invalid-expiry", "900101", "12")]
        [InlineData(1024, "idididi", "1234-1234-1234-1234", "2100-12", "invalid-number", "12")]
        [InlineData(1024, "idididi", "1234-1234-1234-1234", "2100-12", "900101", "invalid")]
        public async Task DoDirectPaymentAsync_throws_ValidationException(
            decimal amount,
            string transactionId,
            string cardNumber,
            string expiry,
            string authentication,
            string partialPassword)
        {
            // arrange
            var aRequest = new DirectPaymentRequest
            {
                Amount = amount,
                TransactionId = transactionId,
                AuthenticationNumber = authentication,
                CardNumber = cardNumber,
                Expiry = expiry,
                PartialPassword = partialPassword,
            };
            var client = Mock.Of<IIamportClient>();
            var sut = new SubscribeApi(client);

            // act/assert
            await Assert.ThrowsAsync<ValidationException>(
                () => sut.DoDirectPaymentAsync(aRequest));
        }

        [Fact]
        public async Task DoDirectPaymentAsync_by_customerId_requests_proper_api()
        {
            // arrange
            var aRequest = new CustomerDirectPaymentRequest
            {
                Amount = 1024,
                TransactionId = Guid.NewGuid().ToString(),
                CustomerId = Guid.NewGuid().ToString(),
            };
            var expectedResult = new IamportResponse<Payment>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new Payment
                {
                    TransactionId = aRequest.TransactionId,
                    Amount = aRequest.Amount,
                }
            };
            var client = GetMockClient(aRequest, expectedResult);
            var sut = new SubscribeApi(client);

            // act
            var result = await sut.DoDirectPaymentAsync(aRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<CustomerDirectPaymentRequest, Payment>(
                        It.Is<IamportRequest<CustomerDirectPaymentRequest>>(req =>
                            req.Method == HttpMethod.Post
                            && req.Content == aRequest
                            && req.ApiPathAndQueryString.EndsWith($"subscribe/payments/again"))));
        }

        [Theory]
        [InlineData(0, null, null)]
        [InlineData(0, "transaction-123", "customer-123")]
        [InlineData(1024, null, "customer-123")]
        [InlineData(1024, "transaction-123", null)]
        [InlineData(1024, "", "customer-123")]
        [InlineData(1024, "transaction-123", "")]
        public async Task DoDirectPaymentAsync_by_customerId_throws_ValidationException(
            decimal amount,
            string transactionId,
            string customerId)
        {
            // arrange
            var aRequest = new CustomerDirectPaymentRequest
            {
                Amount = amount,
                TransactionId = transactionId,
                CustomerId = customerId,
            };
            var client = Mock.Of<IIamportClient>();
            var sut = new SubscribeApi(client);

            // act/assert
            await Assert.ThrowsAsync<ValidationException>(
                () => sut.DoDirectPaymentAsync(aRequest));
        }

        [Fact]
        public async Task SchedulePaymentsAsync_requests_proper_api()
        {
            // arrange
            var aRequest = new SchedulePaymentsRequest
            {
                CustomerId = Guid.NewGuid().ToString(),
                Schedules = new PaymentSchedule[]
                {
                    new PaymentSchedule
                    {
                        Amount = 1024,
                        ScheduleAt = DateTime.UtcNow.AddDays(30),
                        TransactionId = Guid.NewGuid().ToString(),
                    },
                    new PaymentSchedule
                    {
                        Amount = 1024,
                        ScheduleAt = DateTime.UtcNow.AddDays(60),
                        TransactionId = Guid.NewGuid().ToString(),
                    },
                }
            };
            var expectedResult = new IamportResponse<ScheduledPayment[]>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new ScheduledPayment[]
                {
                    new ScheduledPayment
                    {
                        TransactionId = aRequest.Schedules[0].TransactionId,
                        Amount = aRequest.Schedules[0].Amount,
                        ScheduleAt = aRequest.Schedules[0].ScheduleAt,
                    },
                    new ScheduledPayment
                    {
                        TransactionId = aRequest.Schedules[1].TransactionId,
                        Amount = aRequest.Schedules[1].Amount,
                        ScheduleAt = aRequest.Schedules[1].ScheduleAt,
                    },
                }
            };
            var client = GetMockClient(aRequest, expectedResult);
            var sut = new SubscribeApi(client);

            // act
            var result = await sut.SchedulePaymentsAsync(aRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<SchedulePaymentsRequest, ScheduledPayment[]>(
                        It.Is<IamportRequest<SchedulePaymentsRequest>>(req =>
                            req.Method == HttpMethod.Post
                            && req.Content == aRequest
                            && req.ApiPathAndQueryString.EndsWith($"subscribe/payments/schedule"))));
        }

        [Fact]
        public async Task UnschedulePaymentsAsync_requests_proper_api()
        {
            // arrange
            var aRequest = new UnschedulePaymentsRequest
            {
                CustomerId = Guid.NewGuid().ToString(),
                TransactionIds = new string[]
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                },
            };
            var expectedResult = new IamportResponse<ScheduledPayment[]>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Content = new ScheduledPayment[] { }
            };
            var client = GetMockClient(aRequest, expectedResult);
            var sut = new SubscribeApi(client);

            // act
            var result = await sut.UnschedulePaymentsAsync(aRequest);

            // assert
            Mock.Get(client)
                .Verify(mocked =>
                    mocked.RequestAsync<UnschedulePaymentsRequest, ScheduledPayment[]>(
                        It.Is<IamportRequest<UnschedulePaymentsRequest>>(req =>
                            req.Method == HttpMethod.Post
                            && req.Content == aRequest
                            && req.ApiPathAndQueryString.EndsWith($"subscribe/payments/unschedule"))));
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
