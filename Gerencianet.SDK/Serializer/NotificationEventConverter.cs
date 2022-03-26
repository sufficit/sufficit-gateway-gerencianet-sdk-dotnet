using GerencianetSDK.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GerencianetSDK.Serializer
{
    public class NotificationEventConverter : JsonConverter<APIResponse>
    {
        private readonly ILogger _logger;

        public NotificationEventConverter(ILogger logger) : base()
        {
            _logger = logger;
        }

        public override bool HandleNull => false;

        public override APIResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            _logger.LogInformation("reading json notification event");
            return null;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(NotificationEventConverter);
        }

        public override void Write(Utf8JsonWriter writer, APIResponse value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value);
        }
    }
}
