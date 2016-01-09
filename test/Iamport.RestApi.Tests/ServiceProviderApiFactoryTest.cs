using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Iamport.RestApi.Tests
{
    public class ServiceProviderApiFactoryTest
    {
        [Fact]
        public void GuardClause()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ServiceProviderApiFactory(null));
        }

        [Fact]
        public void GetApi_returns_registered_instance_of_api()
        {
            // arrange
            var services = new ServiceCollection();
            services.AddTransient<IDummyApi, DummyApi>();
            var provider = services.BuildServiceProvider();
            var sut = new ServiceProviderApiFactory(provider);

            // act
            var result = sut.GetApi<IDummyApi>();

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetApi_returns_null_for_unknown_type()
        {
            // arrange
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();
            var sut = new ServiceProviderApiFactory(provider);

            // act
            var result = sut.GetApi<IDummyApi>();

            // assert
            Assert.Null(result);
        }

        private interface IDummyApi : IIamportApi
        {
        }
        private class DummyApi : IDummyApi
        {
            public string BasePath { get; } = "/dummy";
        }
    }
}
