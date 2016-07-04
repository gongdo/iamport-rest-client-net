using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;
using System.Reflection;

namespace Iamport.RestApi.Tests.Http
{
    public class JsonContentTest
    {
        [Theory]
        [JsonContentSut(typeof(JsonContent))]
        public void Sut_should_contains_ContentType_header_with_application_json(JsonContent sut)
        {
            // assert
            Assert.Equal("application/json", sut.Headers.ContentType.MediaType);
        }

        [Fact]
        public void Creates_content_with_default_serialization_settings()
        {
            // arrange
            var dummy = Dummy.GetDummy();

            // act
            var sut = new JsonContent(dummy);

            // assert
            var json = sut.ReadAsStringAsync().Result;
            var restored = JsonConvert.DeserializeObject<Dummy>(json);
            Assert.Equal(dummy.Bool, restored.Bool);
            Assert.Equal(dummy.DateTime, restored.DateTime);
            Assert.Equal(dummy.Double, restored.Double);
            Assert.Equal(dummy.Int, restored.Int);
            Assert.Equal(dummy.Long, restored.Long);
            Assert.Equal(dummy.String, restored.String);
        }

        private class Dummy
        {
            public int Int { get; set; }
            public string String { get; set; }
            public DateTime DateTime { get; set; }
            public bool Bool { get; set; }
            public long Long { get; set; }
            public double Double { get; set; }

            public static Dummy GetDummy()
            {
                return new Dummy
                {
                    Bool = true,
                    DateTime = new DateTime(2015, 1, 2, 3, 4, 5, DateTimeKind.Utc),
                    Double = 4.2,
                    Int = 42,
                    Long = 420000000000L,
                    String = "Don't panic. 쫄지마!"
                };
            }
        }

        private class JsonContentSutAttribute : ClassDataAttribute
        {
            public JsonContentSutAttribute(Type @class) : base(@class)
            {
            }

            public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            {
                yield return new[] { new JsonContent(new Dummy()) };
                yield return new[] { new JsonContent(Dummy.GetDummy()) };
                var dummySettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                };
                yield return new[] { new JsonContent(new Dummy(), dummySettings) };
                yield return new[] { new JsonContent(Dummy.GetDummy(), dummySettings) };
                var json = JsonConvert.SerializeObject(new Dummy());
                var dummyJson = JsonConvert.SerializeObject(Dummy.GetDummy());
                yield return new[] { new JsonContent(json) };
                yield return new[] { new JsonContent(dummyJson) };
            }
        }
    }
}
