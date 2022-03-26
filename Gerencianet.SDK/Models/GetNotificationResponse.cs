using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace GerencianetSDK.Models
{
    [DataContract]
    [Serializable]
    public class GetNotificationResponse : APIResponse
    {
        /// <summary>
        /// JSON Content for success or internal error
        /// </summary>
        [DataMember(Name = "data", IsRequired = false, EmitDefaultValue = false, Order = 1)]
        [JsonPropertyName("data"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault), JsonPropertyOrder(1)]
        public new IEnumerable<NotificationEvent> Data { get; set; }
    }
}
