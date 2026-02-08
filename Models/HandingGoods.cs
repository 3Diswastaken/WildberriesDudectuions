using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.Models
{

    public class HandingGoodsRoot
    {
        [JsonPropertyName("data")]
        public HandingGoodsData data { get; set; } = new();
    }
    public class HandingGoodsData
    {
        [JsonPropertyName("goods")]
        public List<HandingGood> Goods { get; set; } = new();
    }
    public class HandingGood : GoodsInterface
    {
        [JsonPropertyName("scanned_code")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Shk { get; set; }
        [JsonPropertyName("on_hold_date")]
        private DateTime? _onHoldDate;
        public DateTime? OnHoldDate 
        { 
            get => _onHoldDate; 
            set => value?.ToLocalTime(); 
        }
        [JsonPropertyName("ticket_id")]
        public long TicketId { get; set; }

        private decimal _price;

        [JsonPropertyName("sum")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Price 
        {
            get => _price; 
            set => _price = value / 100;
        }
        public string Reason { get; set; } = "Зависший ШК";
        public string Pickpoint { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";
    }
}