using System.Diagnostics;
using System.Text;
using System.Text.Json;
using WildberriesDeductions.Wildberries.Models;
using WildberriesDeductions.Wildberries.Exceptions;
using WildberriesDeductions.Wildberries.Models.Dto;

namespace WildberriesDeductions.Wildberries
{
    public class WbRequestService
    {
        private readonly HttpClient _httpClient;

        public WbRequestService(HttpClient http)
        {
            _httpClient = http;
        }

        // заполняем хедеры для конкретного пвз и типа удержаний (зависит от requestConfig)
        private RequestConfig InitializeHeaders(RequestConfig requestConfig, Pickpoint pickpoint)
        {
            RequestConfig clone = requestConfig.Clone();

            clone.Headers["X-PickPoint-External-Id"] = pickpoint.ExternalId.ToString();
            clone.Headers["X-PickPoint-Id"] = pickpoint.Id.ToString();
            clone.Headers["X-Token"] = pickpoint.Token;

            return clone;
        }

        // заполняем плейсхолдеры в url на основе пвз и текущей даты
        private string BuildUrl(RequestConfig requestConfig, Pickpoint pickpoint)
        {
            string url = requestConfig.Url;

            string externalPickpointId = pickpoint.ExternalId.ToString();
            url = url.Replace("{pickpointId}", externalPickpointId);

            string internalPickpointId = pickpoint.Id.ToString();
            url = url.Replace("{internalPickpointId}", internalPickpointId);

            string shard = pickpoint.Shard.ToString();
            url = url.Replace("{shard}", shard);

            // нужно только для defect-goods
            url = url.Replace("{dateTo}", Uri.EscapeDataString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
            url = url.Replace("{dateFrom}", Uri.EscapeDataString(DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
            return url;
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
                throw new NetworkException("Ошибка соединения с сервером. Проверьте доступ к интернету.", ex);
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"[API Error] {url} | Status: {response.StatusCode} | Body: {responseBody}");

                try
                {
                    var errorContent = JsonSerializer.Deserialize<ErrorResponse>(responseBody);
                    if (errorContent?.ErrorInfo != null)
                        throw new ApiException(errorContent.ErrorInfo.Code, errorContent.ErrorInfo.Message);
                }
                catch (JsonException) { } // Если не удалось распарсить тело ошибки, выбрасываем общее исключение

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

        // получение удержаний или тикетов
        public async Task<T> GetGoods<T>(RequestConfig requestConfig, Pickpoint pickpoint)
        {
            string url = BuildUrl(requestConfig, pickpoint);
            var withRuntimeHeaders = InitializeHeaders(requestConfig, pickpoint);

            return await SendRequest<T>(url, requestConfig.Method, requestConfig.Payload, withRuntimeHeaders.Headers);
        }

        // получение 2fa кода
        public async Task<LoginRoot> Login(RequestConfig requestConfig, long phoneNumber)
        {
            var payload = new Dictionary<string, object>(requestConfig.Payload)
            {
                ["phone"] = phoneNumber
            };
            return await SendRequest<LoginRoot>(requestConfig.Url, requestConfig.Method, payload, requestConfig.Headers);
        }

        // валидация 2fa кода
        public async Task<ValidateRoot> Validate(RequestConfig requestConfig, string token, int code)
        {
            var payload = new Dictionary<string, object>(requestConfig.Payload)
            {
                ["token"] = token,
                ["code"] = code.ToString(),
                ["app_version"] = requestConfig.Headers["X-App-Version"]
            };

            return await SendRequest<ValidateRoot>(requestConfig.Url, requestConfig.Method, payload, requestConfig.Headers);
        }


        public async Task<PickpointsListRoot> Pickpoints(RequestConfig requestConfig, string token)
        {
            string url = requestConfig.Url;
            var headers = new Dictionary<string, string>(requestConfig.Headers)
            {
                ["X-Token"] = token
            };

            return await SendRequest<PickpointsListRoot>(url, requestConfig.Method, null, headers);
        }

        // получение нового токена при смене пвз
        public async Task<SwitchRoot> Switch(RequestConfig requestConfig, string token, Pickpoint pickpoint)
        {
            string url = BuildUrl(requestConfig, pickpoint);

            var headers = new Dictionary<string, string>(requestConfig.Headers)
            {
                ["X-Token"] = token
            };

            return await SendRequest<SwitchRoot>(url, requestConfig.Method, null, headers);
        }

        public async Task<WbGoodSearchResponse> FindByShk(RequestConfig requestConfig, Pickpoint pickpoint, string shk)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(shk);
            string shk64base = Convert.ToBase64String(textBytes);

            string url = BuildUrl(requestConfig, pickpoint);
            var withRuntimeHeaders = InitializeHeaders(requestConfig, pickpoint);
            var payload = new Dictionary<string, object>(requestConfig.Payload)
            {
                ["search_base64"] = shk64base
            };

            return await SendRequest<WbGoodSearchResponse>(url, requestConfig.Method, payload, withRuntimeHeaders.Headers);
        }
    }
}
