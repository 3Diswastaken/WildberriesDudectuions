using System.Text.Json;

namespace WildberriesDudectuions
{
    public static class ConfigLoader
    {
        public static RequestConfig Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<RequestConfig>(json);
        }
    }
}
