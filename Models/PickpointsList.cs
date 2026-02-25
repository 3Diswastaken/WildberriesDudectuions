using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.Models
{
    public class PickpointsListRoot
    {
        [JsonPropertyName("data")]
        public List<Pickpoint> Pickpoints { get; set; } = new();
    }

    public class Pickpoint
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("external-id")]
        public int ExternalId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("shard")]
        public int Shard { get; set; }
    }
}
