using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace GerencianetSDK
{
    public class APIResponseError
    {
        [JsonPropertyName("code")]
        public uint Code { get; set; }

        [JsonPropertyName("error")]
        public string ErrorType { get; set; } = default!;

        [JsonPropertyName("error_description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public APIResponseErrorDescription? Description { get; set; }
    }

    public struct APIResponseErrorDescription
    {
        [JsonPropertyName("property")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public string Property { get; set; }

        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }
    }

}
