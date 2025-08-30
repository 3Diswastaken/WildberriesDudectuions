using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDudectuions.Models
{
    public class PickpointId
    {
        [JsonPropertyName("external-id")]
        public string ExternalId { get; set; } = "";
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
    }
    public class UserConfig
    {
        public Dictionary<string, PickpointId> PickpointId { get; set; } = new();
        [JsonPropertyName("X-Token")]
        public string Token { get; set; } = "";
        
    }
}
