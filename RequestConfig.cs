using System.Text.Json;


namespace WildberriesDudectuions
{
    public class RequestConfig
    {
        public string UrlTemplate { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
