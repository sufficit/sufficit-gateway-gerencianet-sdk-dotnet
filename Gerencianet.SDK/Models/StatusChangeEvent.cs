using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GerencianetSDK.Models
{
    [DataContract]
    [Serializable]
    public struct StatusChangeEvent
    {
        [DataMember(Name = "current", IsRequired = true)]
        [JsonPropertyName("current"), JsonPropertyOrder(0)]
        public string Current { get; set; }

        [DataMember(Name = "previous", IsRequired = false, EmitDefaultValue = false)]
        [JsonPropertyName("previous"), JsonPropertyOrder(1), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
        public string? Previous { get; set; }
    }
}
