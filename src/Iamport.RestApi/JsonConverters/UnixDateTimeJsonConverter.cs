using Newtonsoft.Json;
using System;

namespace Iamport.RestApi.JsonConverters
{
    /// <summary>
    /// Unix 시간과 DateTime을 변환하는 JsonConverter.
    /// </summary>
    public class UnixDateTimeJsonConverter : JsonConverter
    {
        /// <inheritedoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime)
                || objectType == typeof(DateTime?);
        }

        /// <inheritedoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            long unixTime = 0;
            if (reader.TokenType == JsonToken.String)
            {
                long.TryParse(reader.Value.ToString(), out unixTime);
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                unixTime = (long)reader.Value;
            }
            else if (reader.TokenType == JsonToken.Date)
            {
                return ((DateTime)reader.Value);
            }
            else if (reader.TokenType == JsonToken.Null)
            {
                unixTime = 0;
            }
            else
            {
                throw new NotSupportedException("DateTime should be string, integer or date.");
            }
            if (unixTime == 0
                && objectType == typeof(DateTime?))
            {
                return null;
            }
            return unixTime.FromUnixTime(includeMilliseconds: false, kind: DateTimeKind.Utc);
        }

        /// <inheritedoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var nullableDateTime = (DateTime?)value;
            DateTime datetime;
            if (nullableDateTime.HasValue)
            {
                datetime = nullableDateTime.Value;
            }
            else
            {
                datetime = (DateTime)value;
            }
            serializer.Serialize(writer, datetime.ToUnixTime(includeMilliseconds: false, kind: DateTimeKind.Utc));
        }
    }
}
