using System.Text.Json.Serialization;

namespace WildberriesDeductions.Models
{
    public class ReplaceGoodsRoot
    {
        [JsonPropertyName("goods")]
        public List<ReplaceGood> Goods { get; set; } = new();
    }

    public class ReplaceGood : GoodsInterface
    {
        public string Reason { get; set; } = "Подмена";
        public string Pickpoint { get; set; } = "";

        [JsonPropertyName("shk")]
        public long Shk { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("on_hold_date")]
        public DateTime? OnHoldDate { get; set; }

        [JsonPropertyName("ticket_id")]
        public long TicketId { get; set; }
    }
}