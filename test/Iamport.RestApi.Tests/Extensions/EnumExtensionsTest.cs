using Iamport.RestApi.Extensions;
using System.Linq;
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

        [Fact]
        public void Returns_EnumMemberMap()
        {
            var map = EnumExtensions.GetMemberValueEnumMap<MyEnum>();
            Assert.Equal("aaa", map.Keys.First());
            Assert.Equal("B", map.Keys.Last());
            Assert.Equal(MyEnum.A, map.Values.First());
            Assert.Equal(MyEnum.B, map.Values.Last());
        }
    }
}
