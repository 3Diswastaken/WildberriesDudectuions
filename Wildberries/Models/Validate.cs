using System.Text.Json.Serialization;

namespace WildberriesDeductions.Wildberries.Models
{
    public class ValidateRoot
    {
        [JsonPropertyName("access")]
        public AccessToken Access { get; set; } = new AccessToken();
    }

    public class AccessToken
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = "";
    }
}
