using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WildberriesDeductions.IPEye.Models;
using WildberriesDeductions.Wildberries.Models;
namespace WildberriesDeductions.IPEye
{
    public class IPEyeRequestService
    {
        private readonly HttpClient _httpClient;

        public IPEyeRequestService(HttpClient http)
        {
            _httpClient = http;
        }

        // низкоуревневый генерик метод отправки запросов
        private async Task<T> SendRequest<T>(string url, string httpMethod, object? payload, Dictionary<string, string>? headers)
        {
            var method = httpMethod?.ToUpper() switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                _ => HttpMethod.Get // Значение по умолчанию
            };

            var request = new HttpRequestMessage(method, url);

            if (method == HttpMethod.Post && payload != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            }

            if (headers != null)
                foreach (var header in headers) request.Headers.TryAddWithoutValidation(header.Key, header.Value);

            HttpResponseMessage response;
            try
            {
                // ловим только сетевые ошибки
                response = await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Network Error] {url}: {ex.Message}");
                throw new Exception("Ошибка соединения с сервером. Проверьте доступ к интернету.", ex);
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            // todo: понять тело ошибки и парсить ошибку
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"[API Error] {url} | Status: {response.StatusCode} | Body: {responseBody}");

                throw new Exception($"Сервер ответил ошибкой {response.StatusCode}: {responseBody}");
            }

            try
            {
                // Десериализуем успешный ответ
                return JsonSerializer.Deserialize<T>(responseBody)
                       ?? throw new Exception("Сервер вернул пустой ответ.");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"[JSON Error] Ошибка парсинга успешного ответа: {ex.Message}");
                throw;
            }

            
        }
        public async Task <LoginDto> Login (RequestConfig requestConfig, string login, string password)
        {
            var payload = new Dictionary<string, object>(requestConfig.Payload)
            {
                ["login"] = login,
                ["pass"] = password
            };
            return await SendRequest<LoginDto>(requestConfig.Url, requestConfig.Method, payload, requestConfig.Headers);
        }
    }
}
