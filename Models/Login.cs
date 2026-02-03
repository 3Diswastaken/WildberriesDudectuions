using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.Models
{
    public class LoginRoot
    {
        [JsonPropertyName("data")]
        public string LoginToken { get; set; } = "";
    }
}
