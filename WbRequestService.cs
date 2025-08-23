using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;


namespace WildberriesDudectuions
{
    public class WbRequestService
    {
        private readonly RequestConfig _config;
        private readonly HttpClient _httpClient;

        public WbRequestService(RequestConfig config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<string> GetDataForPickpoint(string pickpointId)
        {
            string url = _config.UrlTemplate.Replace("{pickpointId}", pickpointId);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            foreach(var header in _config.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
