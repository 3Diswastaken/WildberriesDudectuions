using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDudectuions.Models
{
    public class Switch
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
