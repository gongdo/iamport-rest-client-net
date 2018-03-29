using Iamport.RestApi.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Iamport.RestApi.Tests.Models
{
    public class CreditCardNumberAttributeTest
    {
        class Foo
        {
            [CreditCardNumber]
            public string Number { get; set; }
        }

        [Theory]
        [InlineData("1234-1234-1234-1234")]
        [InlineData("1234-1234-1234-123")]
        [InlineData("1234-1234-1234-12")]
        public void Valid_credit_card_number(string number)
        {
            var foo = new Foo { Number = number };
            var context = new ValidationContext(foo);
            Validator.ValidateObject(foo, context, true);
        }

        [Theory]
        [InlineData("1234123412341234")]
        [InlineData("1234-1234-1234-12345")]
        [InlineData("123-123-123-123")]
        public void Invalid_credit_card_number(string number)
        {
            var foo = new Foo { Number = number };
            var context = new ValidationContext(foo);
            Assert.Throws<ValidationException>(
                () => Validator.ValidateObject(foo, context, true));
        }
    }
}
