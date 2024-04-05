using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace GerencianetSDK.Models
{
    [DataContract]
    [Serializable]
    public class ChargeMetadata
    {
        [DataMember(Name = "notification_url", EmitDefaultValue = false, IsRequired = false)]
        [JsonPropertyName("notification_url"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
        public string? NotificationUrl { get; set; }

        [DataMember(Name = "custom_id", EmitDefaultValue = false, IsRequired = false)]
        [JsonPropertyName("custom_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
        public string? CustomId { get; set; }
    }
}
