using Iamport.RestApi.Apis;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Iamport.RestApi.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Iamport.RestApi.Tests
{
    public class DefaultApiFactoryTest
    {
        [Fact]
        public void GuardClause()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DefaultApiFactory(null));
        }

        [Theory]
        [IamportApiTypesData]
        public void GetApi_returns_an_instance_of_api(Type interfaceType, Type implementationType)
        {
            // arrange
            var client = new DummyClient();
            var sut = new DefaultApiFactory(client);

            // act
            var api = sut.GetApi(interfaceType);

            // assert
            Assert.Equal(implementationType, api.GetType());
        }

        [Fact]
        public void GetApi_returns_null_for_unknown_type()
        {
            // arrange
            var client = new DummyClient();
            var sut = new DefaultApiFactory(client);

            // act
            var api = sut.GetApi(typeof(IFakeApi));

            // assert
            Assert.Null(api);
        }

        private class DummyClient : IIamportHttpClient
        {
            public Task AuthorizeAsync()
            {
                throw new NotImplementedException();
            }

            public Task<IamportResponse<TResult>> RequestAsync<TResult>(HttpRequestMessage request)
            {
                throw new NotImplementedException();
            }

            public Task<IamportResponse<TResult>> RequestAsync<TRequest, TResult>(IamportRequest<TRequest> request)
            {
                throw new NotImplementedException();
            }
        }

        private interface IFakeApi : IIamportApi
        {
        }

        private class IamportApiTypesDataAttribute : ClassDataAttribute
        {
            public IamportApiTypesDataAttribute() : base(typeof(DefaultApiFactory))
            {
            }
            public IamportApiTypesDataAttribute(Type @class) : base(@class)
            {
            }

            public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            {
                yield return new object[] { typeof(IUsersApi), typeof(UsersApi) };
                yield return new object[] { typeof(IPaymentsApi), typeof(PaymentsApi) };
                yield return new object[] { typeof(ISubscribeApi), typeof(SubscribeApi) };
            }
        }
    }
}
