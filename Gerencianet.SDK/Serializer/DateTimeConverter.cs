using GerencianetSDK.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GerencianetSDK.Serializer
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly ILogger _logger;

        public DateTimeConverter(ILogger logger) :base()
        {
            _logger = logger;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            _logger.LogTrace($"reading json datetime: {reader.GetString()}");
            if (reader.TryGetDateTime(out DateTime date))
            {
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }
            else
            {
                var dateString = reader.GetString();
                if (!string.IsNullOrWhiteSpace(dateString))
                {
                    if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out date))
                    {
                        return date;
                    }
                }
            }
            return DateTime.MinValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime());
        }
    }
}
