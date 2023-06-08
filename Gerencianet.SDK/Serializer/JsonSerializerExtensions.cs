using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace GerencianetSDK.Serializer
{
    public static partial class JsonSerializerExtensions
    {
        private static JsonSerializerOptions? _options;
        public static JsonSerializerOptions Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new System.Text.Json.JsonSerializerOptions()
                    {
                        WriteIndented = true,
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString,
                };
                    _options.Converters.Add(new DateTimeConverter());
                }
                return _options;
            }
        }

        public static JsonElement DeserializeAnonymous(string json, JsonSerializerOptions? options = default)
            => JsonSerializer.Deserialize<JsonElement>(json, options ?? Options)!;

        public static T Deserialize<T>(string json, JsonSerializerOptions? options = default)
            => JsonSerializer.Deserialize<T>(json, options ?? Options)!;

        public static ValueTask<TValue> DeserializeAnonymousTypeAsync<TValue>(Stream stream, JsonSerializerOptions? options = default, CancellationToken cancellationToken = default)
            => JsonSerializer.DeserializeAsync<TValue>(stream, options ?? Options, cancellationToken)!; // Method to deserialize from a stream added for completeness
    }
}
