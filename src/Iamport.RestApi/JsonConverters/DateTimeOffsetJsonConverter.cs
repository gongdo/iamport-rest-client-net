using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Iamport.RestApi.JsonConverters
{
    /// <summary>
    /// 아임포트에서 사용하는 날짜 포맷과 DateTimeOffset과의 JsonConverter.
    /// </summary>
    public class DateTimeOffsetJsonConverter : JsonConverter
    {
        /// <summary>
        /// 아임포트의 날짜 포맷
        /// </summary>
        public const string ImportDateFormat = "yyyyMMdd";

        /// <inheritedoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTimeOffset)
                || objectType == typeof(DateTimeOffset?);
        }

        /// <inheritedoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                var text = reader.Value.ToString();
                return string.IsNullOrEmpty(text)
                    ? null
                    : (DateTimeOffset?)DateTimeOffset.ParseExact(text, ImportDateFormat, CultureInfo.InvariantCulture.DateTimeFormat);
            }
            throw new NotSupportedException("Invalid datetime format.");
        }

        /// <inheritedoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);
            }
            else
            {
                var target = (DateTimeOffset)value;
                if (target == null)
                {
                    var nullable = (DateTimeOffset?)value;
                    if (nullable.HasValue == false)
                    {
                        serializer.Serialize(writer, null);
                    }
                    else
                    {
                        serializer.Serialize(writer, nullable.Value.ToString(ImportDateFormat));
                    }
                }
                else
                {
                    serializer.Serialize(writer, target.ToString(ImportDateFormat));
                }
            }
        }
    }
}
