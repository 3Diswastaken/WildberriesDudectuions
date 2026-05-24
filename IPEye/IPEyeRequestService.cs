using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WildberriesDeductions.IPEye.Models;
using WildberriesDeductions.Wildberries.Exceptions;
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

        // Низкоуровневый универсальный метод отправки запросов (заточен под Form URL-Encoded)
        private async Task<T> SendRequest<T>(string url, string httpMethod, Dictionary<string, string>? payload, Dictionary<string, string>? headers)
        {
            var method = httpMethod?.ToUpper() switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                _ => HttpMethod.Get
            };

            var request = new HttpRequestMessage(method, url);

            // Если метод POST — кодируем payload как форму x-www-form-urlencoded
            if (method == HttpMethod.Post && payload != null)
            {
                request.Content = new FormUrlEncodedContent(payload);
            }

            // Добавляем пользовательские заголовки, если они переданы
            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Важно: IPEye критичен к User-Agent. Добавляем "браузерный", если его забыли передать в конфигурации
            if (!request.Headers.Contains("User-Agent"))
            {
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            }

            HttpResponseMessage response;
            try
            {
                // Ловим сетевые ошибки
                response = await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Network Error] {url}: {ex.Message}");
                throw new Exception("Ошибка соединения с сервером. Проверьте доступ к интернету.", ex);
                // Замените на ваш NetworkException, если он объявлен в проекте
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"[API Error] {url} | Status: {response.StatusCode} | Body: {responseBody}");

                try
                {
                    var errorContent = JsonSerializer.Deserialize<ErrorResponse>(responseBody);
                    if (errorContent?.ErrorInfo != null)
                        throw new Exception($"Код: {errorContent.ErrorInfo.Code}. {errorContent.ErrorInfo.Message}");
                    // Замените на ваш ApiException, если нужно
                }
                catch (JsonException) { }

                throw new Exception($"Сервер ответил ошибкой {response.StatusCode}: {responseBody}");
            }

            try
            {
                // Десериализуем успешный ответ (ответ от сервера обычно всё равно JSON)
                return JsonSerializer.Deserialize<T>(responseBody)
                       ?? throw new Exception("Сервер вернул пустой ответ.");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"[JSON Error] Ошибка парсинга успешного ответа: {ex.Message}");
                throw;
            }
        }

        public async Task<LoginDto> Login(RequestConfig requestConfig, string login, string password)
        {
            // Создаем строковый словарь для формы
            var payload = new Dictionary<string, string>();

            // Если в конфиге были базовые параметры payload, копируем их, приводя к string
            if (requestConfig.Payload != null)
            {
                foreach (var item in requestConfig.Payload)
                {
                    payload[item.Key] = item.Value?.ToString() ?? string.Empty;
                }
            }

            // Добавляем или перезаписываем данные авторизации
            payload["login"] = login;
            payload["pass"] = password;

            return await SendRequest<LoginDto>(requestConfig.Url, requestConfig.Method, payload, requestConfig.Headers);
        }

        public async Task<DevicesDtoResponse> Devices(RequestConfig requestConfig)
        {
            var payload = new Dictionary<string, string>();

            // Если в конфиге были базовые параметры payload, копируем их, приводя к string
            if (requestConfig.Payload != null)
            {
                foreach (var item in requestConfig.Payload)
                {
                    payload[item.Key] = item.Value?.ToString() ?? string.Empty;
                }
            }

            return await SendRequest<DevicesDtoResponse>(requestConfig.Url, requestConfig.Method, payload, requestConfig.Headers);
        }
    }
}
