using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private RequestConfig InitializeHeaders(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            RequestConfig clone = requestConfig.Clone();

            var pickpoint = userConfig.PickpointId[pickpointName];

            clone.Headers["X-PickPoint-External-Id"] = pickpoint.ExternalId;
            clone.Headers["X-PickPoint-Id"] = pickpoint.Id;
            clone.Headers["X-Token"] = userConfig.Token;

            return clone;
        }

        // заполняем плейсхолдеры в url на основе пвз и текущей даты
        private string BuildUrl(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string url = requestConfig.Url;

            string externalPickpointId = userConfig.PickpointId[pickpointName].ExternalId;
            url = url.Replace("{pickpointId}", externalPickpointId);

            string internalPickpointId = userConfig.PickpointId[pickpointName].Id;
            url = url.Replace("{internalPickpointId}", internalPickpointId);

            string shard = userConfig.PickpointId[pickpointName].Shard;
            url = url.Replace("{shard}", shard);

            // нужно только для defect-goods
            url = url.Replace("{dateTo}", Uri.EscapeDataString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
            url = url.Replace("{dateFrom}", Uri.EscapeDataString(DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
            return url;
        }
        // низкоуревневый метод отправки запросов
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
        // подмены
        public async Task<string> GetReplaceGoods(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            var withRuntimeHeaders = InitializeHeaders(userConfig, requestConfig, pickpointName);

            return await SendRequest(url, HttpMethod.Get, null, withRuntimeHeaders.Headers);
        }
        // браки
        public async Task<string> GetDefectGoods(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            var withRuntimeHeaders = InitializeHeaders(userConfig, requestConfig, pickpointName);

            return await SendRequest(url, HttpMethod.Get, null, withRuntimeHeaders.Headers);
        }
        // тикеты
        public async Task<string> GetTickets(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            var withRuntimeHeaders = InitializeHeaders(userConfig, requestConfig, pickpointName);

            return await SendRequest(url, HttpMethod.Post, requestConfig.Payload, withRuntimeHeaders.Headers);
        }

        public async Task<string> GetHandingGoods(UserConfig userConfig, RequestConfig requestConfig, string pickpointName)
        {
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            var withRuntimeHeaders = InitializeHeaders(userConfig, requestConfig, pickpointName);
            return await SendRequest(url, HttpMethod.Get, null, withRuntimeHeaders.Headers);
        }
        // получение 2fa кода
        public async Task<string> Login(RequestConfig requestConfig, long phoneNumber)
        {
            var payload = new Dictionary<string, object>(requestConfig.Payload)
            {
                ["phone"] = phoneNumber
            };
            return await SendRequest(requestConfig.Url, HttpMethod.Post, payload, requestConfig.Headers);
        }
        // валидация 2fa кода
        public async Task<string> Validate(RequestConfig requestConfig, string token, int code)
        {
            var payload = new Dictionary<string, object>(requestConfig.Payload)
            {
                ["token"] = token,
                ["code"] = code.ToString()
            };

            return await SendRequest(requestConfig.Url, HttpMethod.Post, payload, requestConfig.Headers);
        }

        // получение нового токена при смене пвз (response code 400)
        public async Task<string> Switch(UserConfig userConfig, RequestConfig requestConfig, string token, string pickpointName)
        {
            string url = BuildUrl(userConfig, requestConfig, pickpointName);
            var headers = new Dictionary<string, string>(requestConfig.Headers)
            {
                ["X-Token"] = token
            };
            Debug.WriteLine($"Switch request url: {url}");


            Debug.WriteLine("Switch request headers:");
            foreach (var header in headers)
            {
                Debug.WriteLine($"{header.Key}: {header.Value}");
            }

            return await SendRequest(url, HttpMethod.Get, null, headers);
        }
    }
}
