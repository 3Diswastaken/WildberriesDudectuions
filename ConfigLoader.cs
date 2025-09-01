using System.Text.Json;
using WildberriesDudectuions.Models;

namespace WildberriesDudectuions
{
    public static class ConfigLoader
    {
        private static readonly string ConfigDirectory = "C:\\Users\\rusla\\source\\repos\\WildberriesDudectuions\\configs\\";
        public static T Load<T>(string fileName)
        {
            string fullPath = Path.Combine(ConfigDirectory, fileName);
            var json = File.ReadAllText(fullPath);
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to deserialize JSON from {fileName}: {ex.Message}", ex);
            }
        }

        public static UserConfig LoadUserConfig() => Load<UserConfig>("user.json");
        public static RequestConfig LoadDefectGoods() => Load<RequestConfig>("defect-goods.json");
        public static RequestConfig LoadHandingGoods() => Load<RequestConfig>("handing-goods.json");
        public static RequestConfig LoadReplaceGoods() => Load<RequestConfig>("replace-goods.json");
        public static RequestConfig LoadTickets() => Load<RequestConfig>("tickets.json");
    }
}
