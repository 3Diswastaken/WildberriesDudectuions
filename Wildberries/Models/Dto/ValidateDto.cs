using System.Text.Json.Serialization;

namespace WildberriesDeductions.Wildberries.Models.Dto
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
