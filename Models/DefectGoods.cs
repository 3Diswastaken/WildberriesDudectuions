using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDudectuions.Models
{
    public class DefectGood : GoodsInterface
    {
        [JsonPropertyName("shk")]
        public long Shk { get; set; }

        [JsonPropertyName("on_hold_date")]
        public DateTime? OnHoldDate { get; set; }

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

    public class DefectGoods
    {
        [JsonPropertyName("defect_products")]
        public List<DefectGood> DefectGoodsList { get; set; } = new();
    }
}