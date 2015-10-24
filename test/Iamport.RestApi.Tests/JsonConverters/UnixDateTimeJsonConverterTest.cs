using Iamport.RestApi.JsonConverters;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Iamport.RestApi.Tests.JsonConverters
{
    public class UnixDateTimeJsonConverterTest
    {
        private readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void Can_convert()
        {
            var sut = new UnixDateTimeJsonConverter();
            var actual = sut.CanConvert(typeof(DateTime));
            Assert.True(actual);
        }

        [Fact]
        public void Cannot_convert()
        {
            var sut = new UnixDateTimeJsonConverter();
            var actual = sut.CanConvert(typeof(DateTimeOffset));
            Assert.False(actual);
        }

        [Fact]
        public void Serializes_correctly()
        {
            var sut = new UnixDateTimeJsonConverter();
            var date = EpochTime;
            var actual = JsonConvert.SerializeObject(date, sut);
            Assert.Equal("0", actual);

            date = DateTime.UtcNow;
            actual = JsonConvert.SerializeObject(date, sut);
            Assert.Equal(((long)((date - EpochTime).TotalSeconds)).ToString(), actual);
        }

        [Fact]
        public void Deserializes_correctly()
        {
            var sut = new UnixDateTimeJsonConverter();
            var date = DateTime.UtcNow;
            var value = ((long)((date - EpochTime).TotalSeconds)).ToString();
            var actual = JsonConvert.DeserializeObject<DateTime>(value, sut);
            Assert.Equal(new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc), actual);

            date = EpochTime;
            actual = JsonConvert.DeserializeObject<DateTime>("0", sut);
            Assert.Equal(date, actual);
        }

        [Fact]
        public void Serializes_object()
        {
            var value = new Dummy
            {
                Value = EpochTime,
            };
            var actual = JsonConvert.SerializeObject(value);
            Assert.Equal("{\"Value\":0}", actual);

            value = new Dummy
            {
                Value = DateTime.UtcNow,
            };
            actual = JsonConvert.SerializeObject(value);
            var unixTime = (long)((value.Value - EpochTime).TotalSeconds);
            Assert.Equal($"{{\"Value\":{unixTime}}}", actual);
        }

        [Fact]
        public void Deserializes_object()
        {
            var value = "{\"Value\":0}";
            var actual = JsonConvert.DeserializeObject<Dummy>(value);
            Assert.Equal(EpochTime, actual.Value);

            var date = DateTime.UtcNow;
            var unixTime = (long)((date - EpochTime).TotalSeconds);
            value = $"{{\"Value\":{unixTime}}}";
            actual = JsonConvert.DeserializeObject<Dummy>(value);
            Assert.Equal(new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc), actual.Value);
        }

        private class Dummy
        {
            [JsonConverter(typeof(UnixDateTimeJsonConverter))]
            public DateTime Value { get; set; }
        }
    }
}
