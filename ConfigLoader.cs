using System.Diagnostics;
using System.Text.Json;
using WildberriesDudectuions.Models;

namespace WildberriesDudectuions
{
    public static class ConfigLoader
    {
        private static readonly string ConfigDirectory = "C:\\Users\\Руслан\\source\\repos\\3Diswastaken\\WildberriesDudectuions\\configs\\";
        private static RequestConfig _commonHeaders;
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

        public static RequestConfig LoadCommonHeaders()
        {
            if (_commonHeaders == null)
            {
                _commonHeaders = Load<RequestConfig>("common-headers.json");
            }
            return _commonHeaders;
        }

        public static RequestConfig LoadRequest(string fileName)
        {
            var common = LoadCommonHeaders();
            var config = Load<RequestConfig>(fileName);

            config.ApplyCommonHeaders(common.Headers);
            return config;
        }

        public static UserConfig LoadUserConfig() => Load<UserConfig>("user.json");

        public static RequestConfig LoadLogin() => LoadRequest("login.json");
        public static RequestConfig LoadValidate() => LoadRequest("validate.json");
        public static RequestConfig LoadDefectGoods() => LoadRequest("defect-goods.json");
        public static RequestConfig LoadHandingGoods() => LoadRequest("handing-goods.json");
        public static RequestConfig LoadReplaceGoods() => LoadRequest("replace-goods.json");
        public static RequestConfig LoadTickets() => LoadRequest("tickets.json");
        public static RequestConfig LoadSwitch() => LoadRequest("switch.json");
    }
}
