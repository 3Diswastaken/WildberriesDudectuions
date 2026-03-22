using System.Text.Json.Serialization;

namespace WildberriesDeductions.Wildberries.Models
{
    public class DefectGoodsRoot
    {
        [JsonPropertyName("defect_products")]
        public List<DefectGood> Goods { get; set; } = new();
    }

    public class DefectGood
    {
        public string Reason { get; set; } = "Брак";
        public string Pickpoint { get; set; } = "";
        [JsonPropertyName("shk")]
        public long Shk { get; set; }

        [JsonPropertyName("on_hold_date")]
        public DateTimeOffset? OnHoldDate { get; set; }

        [JsonPropertyName("retention_price")]
        public decimal Price { get; set; }

        [JsonPropertyName("defect_type")]
        public string DefectType { get; set; } = "";

        [JsonPropertyName("balance_status")]
        public string BalanceStatus { get; set; } = "";

        [JsonPropertyName("ticket_id")]
        public long TicketId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}