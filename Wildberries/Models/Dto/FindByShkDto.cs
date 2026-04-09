using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.Wildberries.Models.Dto
{
    public class WbGoodSearchResponse
    {
        [JsonPropertyName("data")]
        public List<WbGoodSearchEntry> Data { get; set; } = new();
    }

    public class WbGoodSearchEntry
    {
        [JsonPropertyName("good")]
        public WbGoodDetails Details { get; set; } = new();
        [JsonPropertyName("status_history")]
        public List<StatusHistoryItem> StatusHistory { get; set; } = new();
        [JsonPropertyName("defect_mark_data")]
        public List<DefectMarkDataItem> DefectMark { get; set; } = new();
    }

    public class WbGoodDetails
    {
        [JsonPropertyName("info")]
        public WbGoodSpecs Specs { get; set; } = new();
        [JsonPropertyName("cell")]
        public int Cell { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }

    public class WbGoodSpecs
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;
        [JsonPropertyName("goods_size")]
        public string Size { get; set; } = string.Empty;
    }

    public class StatusHistoryItem
    {
        [JsonPropertyName("dt")]
        public DateTimeOffset Date { get; set; }
        [JsonPropertyName("status")]
        public int StatusId { get; set; }
        [JsonPropertyName("status_name")]
        public string StatusName { get; set; } = string.Empty;
        [JsonPropertyName("full_name")]
        public string EmployeeFullName { get; set; } = string.Empty;
    }

    public class DefectMarkDataItem
    {
        [JsonPropertyName("date")]
        public DateTimeOffset Date { get; set; }
        [JsonPropertyName("comment")]
        public string Comment { get; set; } = string.Empty;
        [JsonPropertyName("photos")]
        public List<string> Photos { get; set; } = new();
    }
}
