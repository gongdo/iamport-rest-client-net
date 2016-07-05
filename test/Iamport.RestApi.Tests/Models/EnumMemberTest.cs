using Iamport.RestApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Xunit;

namespace Iamport.RestApi.Tests.Models
{
    public class EnumMemberTest
    {
        [Theory]
        [EnumMemberValueData(typeof(PaymentMethod))]
        [EnumMemberValueData(typeof(PaymentStatus))]
        public void JsonConvert_serializes_enum_to_EnumMember_value(Type enumType, string enumFieldName, string enumValue)
        {
            var enumObject = Enum.Parse(enumType, enumFieldName);
            var serialized = JsonConvert.SerializeObject(enumObject);
            Assert.Equal($"\"{enumValue}\"", serialized);
        }

        [Theory]
        [EnumMemberValueData(typeof(PaymentMethod))]
        [EnumMemberValueData(typeof(PaymentStatus))]
        public void JsonConvert_deserializes_EnumMemver_value_to_enum(Type enumType, string enumFieldName, string enumValue)
        {
            var deserialized = JsonConvert.DeserializeObject($"\"{enumValue}\"", enumType);
            Assert.Equal(enumFieldName, deserialized.ToString());
        }

        private class EnumMemberValueDataAttribute : ClassDataAttribute
        {
            public EnumMemberValueDataAttribute(Type @class) : base(@class)
            {
            }

            public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            {
                var fieldAndValues = Class.GetTypeInfo()
                    .DeclaredFields
                    .Where(e => e.Attributes.HasFlag(FieldAttributes.Literal))
                    .Select(e =>
                    {
                        var enumMember = e.GetCustomAttribute<EnumMemberAttribute>();
                        return new
                        {
                            Field = e,
                            Value = enumMember?.Value
                        };
                    })
                    .Where(e => e.Value != null)
                    .ToList();
                foreach (var fieldAndValue in fieldAndValues)
                {
                    yield return new object[] { Class, fieldAndValue.Field.Name, fieldAndValue.Value };
                }
            }
        }
    }
}
