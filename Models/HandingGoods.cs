using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDudectuions.Models
{
    public class HandingGood : GoodsInterface
    {
        [JsonPropertyName("shk")]
        public long Shk { get; set; }

        [JsonPropertyName("sum")]
        public decimal Price { get; set; }

        [JsonPropertyName("on_hold_date")]
        public DateTime? OnHoldDate { get; set; }

        [JsonPropertyName("ticket_id")]
        public long TicketId { get; set; }
    }
    public class Data
    {
        [JsonPropertyName("goods")]
        public List<HandingGood> HandingGoodsList { get; set; } = new();
    }

    public class Root
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; } = new();
    }

}




