namespace WildberriesDeductions.Wildberries.Models
{
    public class RequestConfig
    {
        public string Url { get; set; } = "";
        public string Method { get; set; } = "GET";
        public Dictionary<string, string> Headers { get; set; } = new();
        public Dictionary<string, object> Payload { get; set; } = new();

        public void ApplyCommonHeaders(Dictionary<string, string> common)
        {
            foreach (var header in common)
                if (!Headers.ContainsKey(header.Key))
                    Headers[header.Key] = header.Value;
        }

        public RequestConfig Clone()
        {
            return new RequestConfig
            {
                Url = this.Url,
                Method = this.Method,
                Headers = new Dictionary<string, string>(this.Headers),
                Payload = new Dictionary<string, object>(this.Payload)
            };
        }
    }
}
