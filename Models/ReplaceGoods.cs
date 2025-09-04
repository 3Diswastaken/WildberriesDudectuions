using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WildberriesDudectuions.Models;

namespace WildberriesDeductions.Models
{
    public class ReplaceGood : GoodsInterface
    {
        [JsonPropertyName("shk")]
        public long Shk { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("on_hold_date")]
        public DateTime? OnHoldDate { get; set; }

        [JsonPropertyName("ticket_id")]
        public long TicketId { get; set; }
    }

    public class ReplaceGoods
    {
        [JsonPropertyName("goods")]
        public List<ReplaceGood> ReplaceGoodsList { get; set; } = new();
    }
}