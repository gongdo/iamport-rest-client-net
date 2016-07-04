using Iamport.RestApi.Apis;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;

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
            var client = Mock.Of<IIamportClient>();
            var sut = new DefaultApiFactory(client);

            // act
            var api = sut.GetApi(interfaceType);

            // assert
            Assert.Equal(implementationType, api.GetType());
        }

        [Theory]
        [InlineData(typeof(IFakeApi))]
        [InlineData(typeof(UsersApi))]
        public void GetApi_returns_null_for_unknown_type(Type type)
        {
            // arrange
            var client = Mock.Of<IIamportClient>();
            var sut = new DefaultApiFactory(client);

            // act
            var api = sut.GetApi(type);

            // assert
            Assert.Null(api);
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
