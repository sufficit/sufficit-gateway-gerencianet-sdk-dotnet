using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace GerencianetSDK.Models
{
    [DataContract]
    [Serializable]
    public class ChargeIdentifier
    {
        [DataMember(Name = "charge_id", IsRequired = false, EmitDefaultValue = false)]
        [JsonPropertyName("charge_id"), JsonPropertyOrder(0), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public uint ChargeId { get; set; }

        [DataMember(Name = "subscription_id", IsRequired = false, EmitDefaultValue = false)]
        [JsonPropertyName("subscription_id"), JsonPropertyOrder(0), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public uint SubscriptionId { get; set; }

        [DataMember(Name = "carnet_id", IsRequired = false, EmitDefaultValue = false)]
        [JsonPropertyName("carnet_id"), JsonPropertyOrder(0), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public uint CarnetId { get; set; }
    }
}
