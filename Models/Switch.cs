using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
