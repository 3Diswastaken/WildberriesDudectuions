using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.IPEye.Models
{
    public class LoginDto
    {
        [JsonPropertyName("danger")]
        public string Danger { get; set; } = string.Empty;
        [JsonPropertyName("success")]
        public string Success { get; set; } = string.Empty;
        [JsonPropertyName("captcha")]
        public string Captcha { get; set; } = string.Empty;
        [JsonPropertyName("check-captcha")]
        public string CheckCaptcha { get; set; } = string.Empty;
        [JsonPropertyName("token")]
        public string Token{ get; set; } = string.Empty;
    }
}