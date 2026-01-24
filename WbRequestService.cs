using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
            //Console.WriteLine(url);
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
                // Console.WriteLine($"{header.Key}   {header.Value}");
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> SendRequest(string url, HttpMethod method, object? payload, Dictionary<string, string>? headers)
        {
            var request = new HttpRequestMessage(method, url);
            if (method == HttpMethod.Post && payload != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            }

            if (headers != null)
                foreach (var header in headers) request.Headers.TryAddWithoutValidation(header.Key, header.Value);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetReplaceGoods(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            
            return await SendRequest(url, HttpMethod.Get, null, requestConfig.Headers);
        }

        public async Task<string> GetDefectGoods(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            return await SendRequest(url, HttpMethod.Get, null, requestConfig.Headers);
        }

        public async Task<string> GetTickets(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            
            return await SendRequest(url, HttpMethod.Post, requestConfig.Payload, requestConfig.Headers);
        }

        public async Task<string> Login(long phoneNumber)
        {
            string url = "https://r-point.wb.ru/api/v1/login";
            string jsonHeaders = File.ReadAllText("C:\\Users\\Руслан\\source\\repos\\3Diswastaken\\WildberriesDudectuions\\configs\\login.json");
            var headersDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonHeaders);

            var payload = new
            {
                phone = phoneNumber,
                resend = false,
                device_uuid = "04aec10c631f41968c69e845b022b455"
            };

            foreach (var header in headersDict)
            {
                Console.WriteLine(header.Key + " " + header.Value);
            }
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            foreach (var header in headersDict)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


    }
}
