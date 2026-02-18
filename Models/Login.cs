using System.Text.Json.Serialization;

namespace WildberriesDeductions.Models
{
    public class LoginRoot
    {
        [JsonPropertyName("data")]
        public string LoginToken { get; set; } = "";
    }
}
