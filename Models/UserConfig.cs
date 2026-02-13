using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.Models
{
    public class PickpointInfo
    {
        [JsonPropertyName("external-id")]
        public string ExternalId { get; set; } = "";
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
        [JsonPropertyName("shard")]
        public string Shard { get; set; } = "";
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
    public class UserConfig
    {
        public Dictionary<string, PickpointInfo> PickpointInfo { get; set; } = new();
        [JsonPropertyName("X-Token")]
        public string Token { get; set; } = "";
    }
}
