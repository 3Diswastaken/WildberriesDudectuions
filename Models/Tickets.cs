using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using WildberriesDudectuions.Models;

namespace WildberriesDeductions.Models
{
    public class TicketHistory
    {
        [JsonPropertyName("create_date")]
        public string CreateDateRaw { get; set; } = "";
        public DateTime? CreateDate => String.IsNullOrEmpty(CreateDateRaw) ? null : DateTimeOffset.Parse(CreateDateRaw).LocalDateTime;
        [JsonPropertyName("state_name")]
        public string StateName { get; set; } = "";
    }

    public class Ticket : GoodsInterface
    {
        public string Reason { get; set; } = "Тикет";
        public string Pickpoint { get; set; } = "";
        public long Shk { get; set; } = 0;
        public DateTime? OnHoldDate
        {
            get 
            {
                if (History?.Count > 0 && History[^1].StateName == "in_progress")
                {
                    return History[^1].CreateDate;
                }
                return null;
            }
            set { }
        }
        [JsonPropertyName("id")]
        public long TicketId { get; set; }
        public decimal Price { get; set; } = 0;
        
        [JsonPropertyName("ticket_history")]
        public List<TicketHistory> History{ get; set; } = new();
    }
    public class TicketsRoot
    {
        [JsonPropertyName("data")]
        public List<Ticket> Tickets { get; set; } = new();
    }
}