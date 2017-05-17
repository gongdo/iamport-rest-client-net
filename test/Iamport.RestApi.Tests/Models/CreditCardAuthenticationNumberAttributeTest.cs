using Iamport.RestApi.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Iamport.RestApi.Tests.Models
{
    public class CreditCardAuthenticationNumberAttributeTest
    {
        class Foo
        {
            [CreditCardAuthenticationNumber]
            public string Number { get; set; }
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("1234567890")]
        public void Valid_expression(string value)
        {
            var foo = new Foo { Number = value };
            var context = new ValidationContext(foo);
            Validator.ValidateObject(foo, context, true);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("1234567")]
        [InlineData("12345678901")]
        public void Invalid_expression(string value)
        {
            var foo = new Foo { Number = value };
            var context = new ValidationContext(foo);
            Assert.Throws<ValidationException>(
                () => Validator.ValidateObject(foo, context, true));
        }
    }
}