using System.Text.Json.Serialization;

namespace WildberriesDeductions.Wildberries.Models.Dto
{
    public class LoginRoot
    {
        [JsonPropertyName("data")]
        public string LoginToken { get; set; } = "";
    }
}
