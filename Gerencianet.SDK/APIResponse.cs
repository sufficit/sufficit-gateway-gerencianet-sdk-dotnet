using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GerencianetSDK
{
    [DataContract][Serializable]
    public class APIResponse
    {
        /// <summary>
        /// HTTP Status Code
        /// </summary>
        [DataMember(Name = "code", IsRequired = true, Order = 0)]
        [JsonPropertyName("code"), JsonPropertyOrder(0)]
        public uint Code { get; set; } = 501;

        /// <summary>
        /// JSON Content for success or internal error
        /// </summary>
        [DataMember(Name = "data", IsRequired = false, EmitDefaultValue = false, Order = 1)]
        [JsonPropertyName("data"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyOrder(1)]
        public virtual JsonElement Data { get; set; }

        /// <summary>
        /// Error message or status description
        /// </summary>        
        [DataMember(Name = "exception", IsRequired = false, EmitDefaultValue = false, Order = 2)]
        [JsonPropertyName("exception"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault), JsonPropertyOrder(2)]
        public string? Exception { get; set; }
    }
}
