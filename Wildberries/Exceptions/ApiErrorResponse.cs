using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.Wildberries.Exceptions
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public ErrorInfo? ErrorInfo { get; set; }
    }
    public class ErrorInfo
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
