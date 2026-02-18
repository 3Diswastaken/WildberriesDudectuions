using System.Text.Json.Serialization;

namespace WildberriesDeductions.Models
{
    public class SwitchRoot
    {
        [JsonPropertyName("access")]
        public AccessSwitchToken Access { get; set; } = new();
    }

    public class AccessSwitchToken
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = "";
    }
}
