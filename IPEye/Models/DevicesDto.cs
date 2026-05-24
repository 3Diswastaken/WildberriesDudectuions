using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.IPEye.Models
{
    public class DevicesDtoResponse
    {
        [JsonPropertyName("data")]
        public List<DeviceDto> Data { get; set; }

        [JsonPropertyName("recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonPropertyName("recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonPropertyName("draw")]
        public int Draw { get; set; }

        [JsonPropertyName("search")]
        public string Search { get; set; } = string.Empty;
    }
    public class DeviceDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("device_name")]
        public string DeviceName { get; set; } = string.Empty;

        [JsonPropertyName("devcode")]
        public string Devcode { get; set; } = string.Empty;

        [JsonPropertyName("storage_server")]
        public string StorageServer { get; set; } = string.Empty;
    }
}