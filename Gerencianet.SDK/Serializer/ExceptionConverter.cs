using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GerencianetSDK.Serializer
{
    public class ExceptionConverter : JsonConverter<Exception?>
    {
        public override Exception? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions _)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.String)
            {
                // reads parsed data as string
                string value = reader.GetString()!;
                return new Exception(value);
            }
            /*
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                string value = reader;
                return new Exception(value);
            }*/

            throw new NotImplementedException($"Unsupported token type: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, Exception? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            writer.WriteString("type", value.GetType().Name);
            writer.WriteString("message", value.Message);

            writer.WriteEndObject();
        }
    }
}
