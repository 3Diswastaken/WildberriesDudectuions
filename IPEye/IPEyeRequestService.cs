using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    string errorJson = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(errorJson);
                }

                string responseJson = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<T>(responseJson);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"ошибка json ответа от сервера: ", ex);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"хуйня какая-то: ", ex);
                throw;
            }
        }
    }
}
