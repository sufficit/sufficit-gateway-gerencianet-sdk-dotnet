using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace GerencianetSDK.Models
{ 
    [DataContract]
    [Serializable]
    public class NotificationEvent
    {
        [DataMember(Name = "id", IsRequired = true)]
        [JsonPropertyName("id"), JsonPropertyOrder(0)]
        public uint Id { get; set; }

        [DataMember(Name = "created_at", IsRequired = true)]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "type", IsRequired = true)]
        [JsonPropertyName("type")]
        public string Type { get; set; }


        [DataMember(Name = "custom_id", EmitDefaultValue = false, IsRequired = false)]
        [JsonPropertyName("custom_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
        public string CustomId { get; set; }

        [DataMember(Name = "value", EmitDefaultValue = false, IsRequired = false)]
        [JsonPropertyName("value"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Value { get; set; }

        [DataMember(Name = "received_by_bank_at", EmitDefaultValue = false, IsRequired = false)]
        [JsonPropertyName("received_by_bank_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]        
        public DateTime ReceivedByBankAt { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false, IsRequired = false)]
        [JsonPropertyName("status"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public StatusChangeEvent Status { get; set; }

        [DataMember(Name = "identifiers", EmitDefaultValue = false, IsRequired = false)]
        [JsonPropertyName("identifiers"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ChargeIdentifier Identifiers { get; set; }
    }
}
