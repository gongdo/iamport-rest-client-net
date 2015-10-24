using Iamport.RestApi.JsonConverters;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Iamport.RestApi.Tests.JsonConverters
{
    public class DateTimeOffsetJsonConverterTest
    {
        [Theory]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(DateTimeOffset?))]
        public void Can_convert(Type objectType)
        {
            var sut = new DateTimeOffsetJsonConverter();
            var actual = sut.CanConvert(objectType);
            Assert.True(actual);
        }

        [Theory]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTime?))]
        public void Cannot_convert(Type objectType)
        {
            var sut = new DateTimeOffsetJsonConverter();
            var actual = sut.CanConvert(objectType);
            Assert.False(actual);
        }

        [Fact]
        public void Serializes_null()
        {
            var sut = new DateTimeOffsetJsonConverter();
            var actual = JsonConvert.SerializeObject(null, sut);
            Assert.Equal("null", actual);
        }

        [Fact]
        public void Serializes_correctly()
        {
            var sut = new DateTimeOffsetJsonConverter();
            var date = DateTimeOffset.Now;
            var actual = JsonConvert.SerializeObject(date, sut);
            Assert.Equal($"\"{date.ToString(DateTimeOffsetJsonConverter.ImportDateFormat)}\"", actual);

            date = DateTimeOffset.UtcNow;
            actual = JsonConvert.SerializeObject(date, sut);
            Assert.Equal($"\"{date.ToString(DateTimeOffsetJsonConverter.ImportDateFormat)}\"", actual);

            date = DateTimeOffset.MinValue;
            actual = JsonConvert.SerializeObject(date, sut);
            Assert.Equal($"\"{date.ToString(DateTimeOffsetJsonConverter.ImportDateFormat)}\"", actual);

            date = DateTimeOffset.MaxValue;
            actual = JsonConvert.SerializeObject(date, sut);
            Assert.Equal($"\"{date.ToString(DateTimeOffsetJsonConverter.ImportDateFormat)}\"", actual);

            var date2 = (DateTimeOffset?)DateTimeOffset.Now;
            actual = JsonConvert.SerializeObject(date2, sut);
            Assert.Equal($"\"{date2.Value.ToString(DateTimeOffsetJsonConverter.ImportDateFormat)}\"", actual);

            date2 = (DateTimeOffset?)DateTimeOffset.UtcNow;
            actual = JsonConvert.SerializeObject(date2, sut);
            Assert.Equal($"\"{date2.Value.ToString(DateTimeOffsetJsonConverter.ImportDateFormat)}\"", actual);

            date2 = (DateTimeOffset?)DateTimeOffset.MinValue;
            actual = JsonConvert.SerializeObject(date2, sut);
            Assert.Equal($"\"{date2.Value.ToString(DateTimeOffsetJsonConverter.ImportDateFormat)}\"", actual);

            date2 = (DateTimeOffset?)DateTimeOffset.MaxValue;
            actual = JsonConvert.SerializeObject(date2, sut);
            Assert.Equal($"\"{date2.Value.ToString(DateTimeOffsetJsonConverter.ImportDateFormat)}\"", actual);
        }

        [Fact]
        public void Deserializes_correctly()
        {
            var sut = new DateTimeOffsetJsonConverter();
            var value = "\"19700101\"";
            var actual = JsonConvert.DeserializeObject<DateTimeOffset>(value, sut);
            Assert.Equal(DateTimeOffset.Parse("1970-01-01"), actual);

            value = "\"29991231\"";
            actual = JsonConvert.DeserializeObject<DateTimeOffset>(value, sut);
            Assert.Equal(DateTimeOffset.Parse("2999-12-31"), actual);

            value = "\"19700101\"";
            var actual2 = JsonConvert.DeserializeObject<DateTimeOffset?>(value, sut);
            Assert.Equal(DateTimeOffset.Parse("1970-01-01"), actual2.Value);

            value = "\"29991231\"";
            actual2 = JsonConvert.DeserializeObject<DateTimeOffset?>(value, sut);
            Assert.Equal(DateTimeOffset.Parse("2999-12-31"), actual2.Value);
        }

        [Fact]
        public void Serializes_object()
        {
            var value = new Dummy
            {
                Value = DateTimeOffset.Parse("1970-01-01"),
                Nullable = DateTimeOffset.Parse("1970-01-01"),
            };
            var actual = JsonConvert.SerializeObject(value);
            Assert.Equal("{\"Value\":\"19700101\",\"Nullable\":\"19700101\"}", actual);
        }

        [Fact]
        public void Deserializes_object()
        {
            var value = "{\"Value\":\"19700101\",\"Nullable\":\"19700101\"}";
            var actual = JsonConvert.DeserializeObject<Dummy>(value);
            Assert.Equal("19700101", actual.Value.ToString(DateTimeOffsetJsonConverter.ImportDateFormat));
            Assert.Equal("19700101", actual.Nullable.Value.ToString(DateTimeOffsetJsonConverter.ImportDateFormat));
        }

        private class Dummy
        {
            [JsonConverter(typeof(DateTimeOffsetJsonConverter))]
            public DateTimeOffset Value { get; set; }
            [JsonConverter(typeof(DateTimeOffsetJsonConverter))]
            public DateTimeOffset? Nullable { get; set; }
        }
    }
}
