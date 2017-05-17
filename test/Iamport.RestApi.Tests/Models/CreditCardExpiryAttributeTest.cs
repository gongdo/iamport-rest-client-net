using Iamport.RestApi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Iamport.RestApi.Tests.Models
{
    public class CreditCardExpiryAttributeTest
    {
        class Foo
        {
            [CreditCardExpiry]
            public string Expiry { get; set; }
        }

        [Fact]
        public void Valid_expression()
        {
            var value = DateTimeOffset.Now.AddMonths(1).ToString("yyyy-MM");
            var foo = new Foo { Expiry = value };
            var context = new ValidationContext(foo);
            Validator.ValidateObject(foo, context, true);
        }

        [Theory]
        [InlineData("123-12")]
        [InlineData("1234-123")]
        [InlineData("2017-1")]
        [InlineData("2017-01")]
        public void Invalid_expression(string value)
        {
            var foo = new Foo { Expiry = value };
            var context = new ValidationContext(foo);
            Assert.Throws<ValidationException>(
                () => Validator.ValidateObject(foo, context, true));
        }
    }
}
