using System.Runtime.InteropServices;
using System.Text.Json;


namespace WildberriesDudectuions.Models
{
    public class RequestConfig
    {
        public string Url { get; set; } = "";
        public string Method { get; set; } = "GET";
        public Dictionary<string, string> Headers { get; set; } = new();
        public Dictionary<string, object> Payload { get; set; } = new();

        public void ApplyCommonHeaders(Dictionary<string, string> common)
        {
            foreach (var h in common)
                if (!Headers.ContainsKey(h.Key))
                    Headers[h.Key] = h.Value;
        }
    }
}
