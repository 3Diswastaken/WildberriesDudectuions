using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using WildberriesDeductions.Models;

namespace WildberriesDeductions.Models
{
    public class TicketsRoot
    {
        [JsonPropertyName("data")]
        public List<Ticket> Tickets { get; set; } = new();
    }

    public class Ticket : GoodsInterface
    {
        [JsonPropertyName("support_logs")]
        public List<TicketHistory> Logs { get; set; } = new();


        public string Reason { get; set; } = "Тикет";
        public string Pickpoint { get; set; } = "";
        public long Shk { get; set; } = 0;
        public DateTime? OnHoldDate
        {
            get 
            {
                if (Logs?.Count > 0 && Logs[^1].FullName.Trim() != "Руслан")
                {
                    return Logs[^1].CreateDate!.Value.AddDays(2);
                }
                return null;
            }
            set { }
        }
        [JsonPropertyName("id")]
        public long TicketId { get; set; }
        public decimal Price { get; set; } = 0;
        [JsonPropertyName("ticket_state_id")]
        public int State { get; set; }
    }
    public class TicketHistory
    {
        [JsonPropertyName("create_date")]
        public string CreateDateRaw { get; set; } = "";
        public DateTime? CreateDate => String.IsNullOrEmpty(CreateDateRaw) ? null : DateTimeOffset.Parse(CreateDateRaw).LocalDateTime;
        [JsonPropertyName("fio")]
        public string FullName { get; set; } = "";
    }
}