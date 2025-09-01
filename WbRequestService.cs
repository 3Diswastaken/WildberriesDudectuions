using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using WildberriesDudectuions.Models;


namespace WildberriesDudectuions
{
    public class WbRequestService
    {
        private readonly HttpClient _httpClient;

        public WbRequestService(HttpClient http)
        {
            _httpClient = http;
        }

        // заполняем хедеры для конкретного пвз и типа удержаний (зависит от requestConfig)
        private void InitializeHeaders(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            var pickpoint = userConfig.PickpointId[pickpointName];

            requestConfig.Headers["X-PickPoint-External-Id"] = pickpoint.ExternalId;
            requestConfig.Headers["X-PickPoint-Id"] = pickpoint.Id;
            requestConfig.Headers["X-Token"] = userConfig.Token;
        }

        // заполняем плейсхолдеры в url на основе пвз и текущей даты
        private string BuildUrl(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string pickpointId = userConfig.PickpointId[pickpointName].ExternalId;
            string url = requestConfig.Url;
            url = url.Replace("{pickpointId}", pickpointId);

            // нужно только для defect-goods
            url = url.Replace("{dateTo}", Uri.EscapeDataString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
            url = url.Replace("{dateFrom}", Uri.EscapeDataString(DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
            
            return url;
        }

        // метод для получения данных по конкретному пвз и типу удержаний (зависит от requestConfig)
        public async Task<string> GetDataForPickpoint(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            InitializeHeaders(userConfig, requestConfig, pickpointName);
            HttpRequestMessage request;

            // создаем запрос в зависимости от метода
            if (requestConfig.Method == "POST")
            {
                request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestConfig.Payload), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Get, url);
            }

            foreach (var header in requestConfig.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
