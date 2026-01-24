using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDudectuions.Models
{
    public class Validate
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
