using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using WildberriesDeductions.Models;


namespace WildberriesDeductions
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
                ["code"] = code.ToString()
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
            string url = BuildUrl(requestConfig, pickpoint);
            var withRuntimeHeaders = InitializeHeaders(requestConfig, pickpoint);
            var payload = new Dictionary<string, object>(requestConfig.Payload)
            {
                ["search_base64"] = shk
            };

            return await SendRequest<WbGoodSearchResponse>(url, requestConfig.Method, payload, withRuntimeHeaders.Headers);
        }
    }
}
