using Iamport.RestApi.Extensions;
using System.Runtime.Serialization;
using Xunit;

namespace Iamport.RestApi.Tests.Extensions
{
    public class EnumExtensionsTest
    {
        private enum MyEnum
        {
            [EnumMember(Value = "aaa")]
            A,
            B,
        }

        [Fact]
        public void Returns_Value_of_EnumMember()
        {
            var actual = MyEnum.A.GetMemberValue();
            Assert.Equal("aaa", actual);
        }

        [Fact]
        public void Returns_default_enum_string()
        {
            var actual = MyEnum.B.GetMemberValue();
            Assert.Equal("B", actual);
        }
    }
}
